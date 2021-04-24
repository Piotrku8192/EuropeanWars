using EuropeanWars.Audio;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Data;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Pathfinding;
using EuropeanWars.Core.Persons;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Time;
using EuropeanWars.Network;
using EuropeanWars.UI.Windows;
using Lidgren.Network;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.Core.Army {
    public class ArmyInfo {
        public static List<ArmyInfo> selectedArmies = new List<ArmyInfo>();
        private static int nextId;
        public readonly int id;

        public readonly bool isNavy;

        public Dictionary<UnitInfo, int> units = new Dictionary<UnitInfo, int>();
        public Dictionary<UnitInfo, int> maxUnits = new Dictionary<UnitInfo, int>();

        public int Size => units.Values.Sum();
        public int MaxSize => maxUnits.Values.Sum();
        public int Artilleries => GetArtilleries();
        public float AverageSpeed => (units.Sum(t => t.Key.speed * t.Value) * GameStatistics.armySpeedModifier) / (Size > 0 ? Size : 1) * Country.armySpeedModifier;
        public int Maintenance => Mathf.RoundToInt(units.Sum(t => t.Key.maintenance * t.Value));
        public int FoodPerMonth => units.Sum(t => t.Key.foodPerMonth * t.Value / t.Key.recruitSize);

        public int NavyCapacity => units.Sum(t => t.Key.type == UnitType.Navy ? t.Key.recruitSize : 0);

        public bool IsSelected { get; private set; }

        public bool isMoveLocked;
        public int movingforDays;
        public int daysToMove;

        public CountryInfo Country { get; private set; }
        public ProvinceInfo Province { get; private set; }
        public ArmyObject ArmyObject { get; private set; }
        public bool BlackStatus { get; private set; }

        public General General { get; private set; }

        public Queue<ProvinceInfo> route = new Queue<ProvinceInfo>();

        public ArmyInfo(ProvinceInfo province, CountryInfo country, UnitInfo unit, int count, int maxCount, bool selectAsMovingArmy = false) {
            isNavy = unit.type == UnitType.Navy;
            units.Add(unit, count);
            maxUnits.Add(unit, maxCount);
            Country = country;
            Province = province;
            Country.armies.Add(this);
            Province.armies.Add(this);
            id = nextId;
            GameInfo.armies.Add(id, this);
            nextId++;

            ArmyObject = ArmySpawner.Singleton.SpawnAndInitializeArmy(this);
            TimeManager.onDayElapsed += CountMovement;
            TimeManager.onDayElapsed += UpdateBlackStatus;
            TimeManager.onMonthElapsed += ReinforceArmy;

            if (selectAsMovingArmy) {
                ArmyInfo a = SelectedArmyWindow.Singleton.SelectedArmy;
                SelectArmy(false);
                SelectedArmyWindow.Singleton.SelectArmy(a);
                SelectedArmyWindow.Singleton.SelectMovingArmy(this);
            }
        }

        public ArmyInfo(UnitToRecruit unit) {
            isNavy = unit.unitInfo.type == UnitType.Navy;
            units.Add(unit.unitInfo, unit.count * (unit.unitInfo.type == UnitType.Navy ? 1 : unit.unitInfo.recruitSize));
            maxUnits.Add(unit.unitInfo, unit.count * (unit.unitInfo.type == UnitType.Navy ? 1 : unit.unitInfo.recruitSize));
            Country = unit.country; 
            Province = unit.province;
            Country.armies.Add(this);
            Province.armies.Add(this);
            id = nextId;
            GameInfo.armies.Add(id, this);
            nextId++;

            ArmyObject = ArmySpawner.Singleton.SpawnAndInitializeArmy(this);
            TimeManager.onDayElapsed += CountMovement;
            TimeManager.onDayElapsed += UpdateBlackStatus;
            TimeManager.onMonthElapsed += ReinforceArmy;
        }

        public ArmyInfo(ArmyData army) {
            isNavy = army.isNavy;
            for (int i = 0; i < army.unitsT.Length; i++) {
                units.Add(GameInfo.units[army.unitsT[i]], army.unitsS[i]);
            }
            for (int i = 0; i < army.maxUnitsT.Length; i++) {
                maxUnits.Add(GameInfo.units[army.maxUnitsT[i]], army.maxUnitsS[i]);
            }
            Country = GameInfo.countries[army.country];
            Province = GameInfo.provincesByColor[army.province];

            foreach (var item in army.route) {
                route.Enqueue(GameInfo.provincesByColor[item]);
            }
            BlackStatus = army.blackStatus;
            isMoveLocked = army.isMovingLocked;

            Country.armies.Add(this);
            Province.armies.Add(this);
            id = nextId;
            GameInfo.armies.Add(id, this);
            nextId++;

            ArmyObject = ArmySpawner.Singleton.SpawnAndInitializeArmy(this);
            TimeManager.onDayElapsed += CountMovement;
            TimeManager.onDayElapsed += UpdateBlackStatus;
            TimeManager.onMonthElapsed += ReinforceArmy;
        }

        public void SetCountry(CountryInfo country) {
            Country.armies.Remove(this);
            Country = country;
            country.armies.Add(this);
        }

        private void CountMovement() {
            if (route.Count > 1) {
                movingforDays++;
                if (movingforDays >= daysToMove) {
                    route.Dequeue();
                    ProvinceInfo[] ra = route.ToArray();
                    movingforDays = 0;
                    daysToMove = 0;
                    OnArmyMove(ra[0]);
                    if (ra.Length > 1) {
                        daysToMove = Mathf.CeilToInt(Vector2.Distance(
                            new Vector2(ra[0].x, ra[0].y),
                            new Vector2(ra[1].x, ra[1].y)) / (AverageSpeed == 0 ? 1 : AverageSpeed));
                    }
                }
            }
        }

        private void ReinforceArmy() {
            CalculateFood();

            if (Province.NationalCountry == Country) {
                int avaiable = Mathf.Clamp(Province.taxation * GameStatistics.provinceIncomeArmyReinforcementModifier, 0, Country.manpower);
                UnitInfo[] ks = units.Keys.ToArray();
                foreach (var item in ks) {
                    if (units[item] < maxUnits[item]) {
                        int i = Mathf.Clamp(maxUnits[item] - units[item], 0, avaiable);
                        units[item] += i;
                        avaiable -= i;
                        Country.manpower -= i;
                    }

                    if (avaiable == 0) {
                        break;
                    }
                }
            }
        }

        private void CalculateFood() {
            int foodToGive = 0;
            int f = Mathf.Max(Country.food, FoodPerMonth);

            if (Province.Country == Country) {
                foodToGive = Mathf.Min(Country.food, FoodPerMonth);
                Country.food = foodToGive < FoodPerMonth ? 0 : Country.food - FoodPerMonth;
            }
            foodToGive += units.Sum(t => t.Key.foodCapacity * t.Value / t.Key.recruitSize);

            int usedFood = 0;
            foreach (UnitInfo item in units.Keys.ToArray()) {
                if (foodToGive >= item.foodPerMonth) {
                    foodToGive -= item.foodPerMonth;
                    usedFood += item.foodPerMonth;
                }
                else {
                    units[item] = Mathf.Clamp(units[item] - maxUnits[item] / 10, 0, int.MaxValue); //10% per month
                }
            }

            usedFood -= f;
            foreach (UnitInfo item in units.Keys.ToArray()) {
                if (usedFood >= item.foodCapacity && item.foodCapacity > 0) {
                    usedFood -= item.foodCapacity;
                    RemoveUnit(item, item.recruitSize);
                }
                else {
                    break;
                }
            }
        }

        private void UpdateBlackStatus() {
            BlackStatus = Country != Province.Country && !Country.relations[Province.Country].relations[(int)DiplomaticRelation.MilitaryAccess]
                && !Country.IsInWarAgainstCountry(Province.Country);
            if (route.Count < 2) {
                isMoveLocked = false;
            }
        }

        private int GetArtilleries() {
            int i = 0;
            foreach (var item in units) {
                if (item.Key.type == UnitType.Artillery) {
                    i += Mathf.CeilToInt(item.Value / (float)item.Key.recruitSize);
                }
            }

            return i;
        }

        public static void UnselectAll() {
            foreach (var item in new List<ArmyInfo>(selectedArmies)) {
                item.UnselectArmy();
            }
        }

        public void Delete() {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)2052);
            msg.Write(id);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        public void DeleteLocal() {
            UnselectArmy();
            GameInfo.armies.Remove(id);
            Country.armies.Remove(this);
            Province.armies.Remove(this);
            if (General != null) {
                General.army = null;
            }
            TimeManager.onDayElapsed -= CountMovement;
            TimeManager.onDayElapsed -= UpdateBlackStatus;
            TimeManager.onMonthElapsed -= ReinforceArmy;
            try {
                ArmyObject.StopAllCoroutines();
                ArmySpawner.Singleton.DestroyArmy(ArmyObject);
            }
            catch {

            }
            Province.RefreshFogOfWarInRegion();
        }

        public void MoveUnitToOtherArmyRequest(UnitInfo unit, ArmyInfo targetArmy, int count) {
            if (targetArmy == null || Province == targetArmy.Province) {
                int c = units[unit];
                int mc = maxUnits[unit];
                RemoveUnitRequest(unit, count);

                if (targetArmy == null) {
                    NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                    msg.Write((ushort)2053);
                    msg.Write(Country.id);
                    msg.Write(Province.id);
                    msg.Write(unit.id);
                    msg.Write(Mathf.Clamp(count, 0, c));
                    msg.Write(Mathf.Clamp(count, 0, mc));
                    msg.Write(SelectedArmyWindow.Singleton.SelectedArmy == this);
                    Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                }
                else {
                    targetArmy.AddUnitRequest(unit, Mathf.Clamp(count, 0, c), Mathf.Clamp(count, 0, mc));
                }
            }
        }

        public void MoveUnitToOtherArmy(UnitInfo unit, ArmyInfo targetArmy, int count) {
            if (targetArmy.isNavy) {
                int usedCapacity = targetArmy.units.Sum(t => t.Key.type != UnitType.Navy ? t.Value : 0);
                count = Mathf.Clamp(count, 0, targetArmy.NavyCapacity - usedCapacity);
            }

            if (Province == targetArmy.Province && targetArmy != null) {
                int c = units[unit];
                int mc = maxUnits[unit];
                RemoveUnit(unit, count);
                targetArmy.AddUnit(unit, Mathf.Clamp(count, 0, c), Mathf.Clamp(count, 0, mc));
            }
        }

        public void AddUnitRequest(UnitInfo unit, int count, int maxCount) {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)2050);
            msg.Write(id);
            msg.Write(unit.id);
            msg.Write(count);
            msg.Write(maxCount);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        public void AddUnit(UnitInfo unit, int count, int maxCount) {
            if (isNavy) {
                int usedCapacity = units.Sum(t => t.Key.type != UnitType.Navy ? t.Value : 0);
                count = Mathf.Clamp(count, 0, NavyCapacity - usedCapacity);
            }

            if (!units.ContainsKey(unit)) {
                maxUnits.Add(unit, maxCount);
                units.Add(unit, count);
            }
            else {
                maxUnits[unit] += maxCount;
                units[unit] += count;
            }

            if (selectedArmies.Contains(this)) {
                SelectedArmyWindow.Singleton.UpdateWindow();
            }
        }

        public void RemoveUnitRequest(UnitInfo unit, int count) {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)2051);
            msg.Write(id);
            msg.Write(unit.id);
            msg.Write(count);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        public void RemoveUnit(UnitInfo unit, int count) {
            if (units.ContainsKey(unit) && maxUnits.ContainsKey(unit)) {
                if (count >= maxUnits[unit]) {
                    units.Remove(unit);
                    maxUnits.Remove(unit);
                    if (units.Count == 0) {
                        DeleteLocal();
                    }

                    if (selectedArmies.Contains(this)) {
                        SelectedArmyWindow.Singleton.UpdateWindow();
                    }
                }
                else {
                    units[unit] -= Mathf.Clamp(count, 0, units[unit]);
                    maxUnits[unit] -= count;
                }
            }
        }

        public void SelectArmy(bool unselectOthers = true) {
            if (selectedArmies.Contains(this)) {
                return;
            }
            if (Country == GameInfo.PlayerCountry) {
                if (unselectOthers) {
                    UnselectAll();
                }
                selectedArmies.Add(this);
                IsSelected = true;
                ArmyObject.selectionOutline.gameObject.SetActive(true);
                SelectedArmyWindow.Singleton.AddArmy(this);
                MusicManager.Singleton.audioEffectsSource.clip = MusicManager.Singleton.effects[0];
                MusicManager.Singleton.audioEffectsSource.Play();
            }
        }

        public void UnselectArmy() {
            if (selectedArmies.Contains(this)) {
                selectedArmies.Remove(this);
                IsSelected = false;
                ArmyObject.selectionOutline.gameObject.SetActive(false);
                SelectedArmyWindow.Singleton.RemoveArmy(this);
            }
        }

        /// <summary>
        /// Only for player
        /// </summary>
        /// <param name="target"></param>
        public void GenerateRouteRequest(ProvinceInfo target) {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)2049);
            msg.Write(id);
            msg.Write(target.id);

            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Only for bots and NetworkClient
        /// </summary>
        /// <param name="target"></param>
        public void GenerateRoute(ProvinceInfo target) {
            if (isMoveLocked) {
                return;
            }
            if (target == Province) {
                route.Clear();
                route.Enqueue(target);
                if (Country == GameInfo.PlayerCountry) {
                    ArmyObject.DrawRoute(route.ToArray());
                }
                ArmyObject.StopAllCoroutines();
                ArmyObject.transform.position = new Vector3(Province.x, Province.y);
                ArmyObject.isMovementCoroutineExecuting = false;

                return;
            }

            ArmyPathfinder pathfinder = new ArmyPathfinder(this);
            ProvinceInfo[] r = pathfinder.FindPath(target);
            if (r != null && r.Length > 1) {
                route.Clear();
                foreach (var item in r) {
                    route.Enqueue(item);
                }
                if (Country == GameInfo.PlayerCountry) {
                    ArmyObject.DrawRoute(route.ToArray());
                }

                daysToMove = Mathf.CeilToInt(Vector2.Distance(
                    new Vector2(Province.x, Province.y),
                    new Vector2(r[1].x, r[1].y)) / (AverageSpeed == 0 ? 1 : AverageSpeed));
                movingforDays = 0;
            }
        }

        public void OnArmyMove(ProvinceInfo newProvince) {
            ArmyObject.StartCoroutine(ArmyObject.MoveObjectToProvince(Province, newProvince));

            if (route.Count < 2) {
                isMoveLocked = false;
            }

            Province.armies.Remove(this);
            newProvince.armies.Add(this);

            if (Country == GameInfo.PlayerCountry || GameInfo.PlayerCountry.relations[Country].relations[(int)DiplomaticRelation.Alliance]) {
                Province.RefreshFogOfWarInRegion();
                newProvince.SetFogOfWarInRegion(false);
            }

            Province = newProvince;

            var a = Province.armies.Where(t => t.Country.IsInWarAgainstCountry(Country)); //TODO: Add uprisings
            if (a.Count() > 0) {
                new Battle(new BattleArmyGroup(new ArmyInfo[1] { this }), new BattleArmyGroup(a.ToArray()), Province);
            }
        }

        public void SetGeneralRequest(General general) {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)2054);
            msg.Write(id);
            msg.Write(general == null ? -1 : general.id);

            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        public void SetGeneral(General general) {
            if (General != null && General != general) {
                General.army = null;
            }
            General = general;
            if (general != null) {
                General.army = this;
            }

            if (SelectedArmyWindow.Singleton.SelectedArmy == this) {
                SelectedArmyWindow.Singleton.UpdateWindow();
            }
        }
    }
}