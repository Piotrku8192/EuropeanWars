using EuropeanWars.Core.Country;
using EuropeanWars.Core.Pathfinding;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Time;
using EuropeanWars.Network;
using EuropeanWars.UI.Windows;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.Core.Army {
    public class ArmyInfo {
        public static List<ArmyInfo> selectedArmies = new List<ArmyInfo>();
        private static int nextId;
        public readonly int id;

        public Dictionary<UnitInfo, int> units = new Dictionary<UnitInfo, int>();
        public Dictionary<UnitInfo, int> maxUnits = new Dictionary<UnitInfo, int>();

        public int Size => units.Values.Sum();
        public int MaxSize => maxUnits.Values.Sum();
        public int Artilleries => GetArtilleries();
        public int AverageSpeed => units.Sum(t => t.Key.speed * t.Value) / (Size > 0 ? Size : 1);
        public int Maintenance => Mathf.RoundToInt(units.Sum(t => t.Key.maintenance * t.Value));
        public bool IsSelected { get; private set; }

        public CountryInfo Country { get; private set; }
        public ProvinceInfo Province { get; private set; }
        public ArmyObject ArmyObject { get; private set; }
        public bool BlackStatus { get; private set; }

        public Queue<ProvinceInfo> route = new Queue<ProvinceInfo>();


        public ArmyInfo(UnitToRecruit unit) {
            units.Add(unit.unitInfo, unit.count * unit.unitInfo.recruitSize);
            maxUnits.Add(unit.unitInfo, unit.count * unit.unitInfo.recruitSize);
            Country = unit.country;
            Province = unit.province;
            Country.armies.Add(this);
            Province.armies.Add(this);
            id = nextId;
            GameInfo.armies.Add(id, this);
            nextId++;

            ArmyObject = ArmySpawner.Singleton.SpawnAndInitializeArmy(this);
            TimeManager.onDayElapsed += ArmyObject.CountMovement;
            TimeManager.onDayElapsed += UpdateBlackStatus;
            TimeManager.onMonthElapsed += ReinforcementArmy;
        }

        ~ArmyInfo() {
            TimeManager.onDayElapsed -= UpdateBlackStatus;
            TimeManager.onMonthElapsed -= ReinforcementArmy;
        }

        private void ReinforcementArmy() {
            if (Province.claimators.Contains(Country)) {
                int avaiable = Mathf.Clamp(Province.taxation * 100, 0, Country.manpower);
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

        private void UpdateBlackStatus() {
            BlackStatus = Country != Province.Country && !Country.militaryAccesses.ContainsKey(Province.Country) && !Country.IsInWarAgainstCountry(Province.Country);
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
            TimeManager.onDayElapsed -= ArmyObject.CountMovement;
            TimeManager.onDayElapsed -= UpdateBlackStatus;
            TimeManager.onMonthElapsed -= ReinforcementArmy;
            ArmyObject.StopAllCoroutines();
            ArmySpawner.Singleton.DestroyArmy(ArmyObject);
        }

        public void MoveUnitToOtherArmy(UnitInfo unit, ArmyInfo targetArmy, int count) {
            if (targetArmy != null && Province == targetArmy.Province) {
                int c = units[unit];
                int mc = maxUnits[unit];
                RemoveUnitRequest(unit, count);
                targetArmy.AddUnitRequest(unit, Mathf.Clamp(count, 0, c), Mathf.Clamp(count, 0, mc));
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
                        Delete();
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
            LandArmyPathfinder pathfinder = new LandArmyPathfinder(this);

            if (target == Province) {
                route.Clear();
                route.Enqueue(target);
                if (Country == GameInfo.PlayerCountry) {
                    ArmyObject.DrawRoute(route.ToArray());
                }
                ArmyObject.StopAllCoroutines();
                ArmyObject.transform.position = new Vector3(Province.x, Province.y);
                return;
            }
            ProvinceInfo[] r = pathfinder.FindPath(target);
            if (r != null) {
                route.Clear();
                foreach (var item in r) {
                    route.Enqueue(item);
                }
                if (Country == GameInfo.PlayerCountry) {
                    ArmyObject.DrawRoute(route.ToArray());
                }
            }
        }

        public void OnArmyMove(ProvinceInfo newProvince) {
            Province.armies.Remove(this);
            newProvince.armies.Add(this);

            if (Country == GameInfo.PlayerCountry || GameInfo.PlayerCountry.alliances.ContainsKey(Country)) {
                Province.RefreshFogOfWarInRegion();
                newProvince.SetFogOfWarInRegion(false);
            }

            Province = newProvince;

            var a = Province.armies.FirstOrDefault(t => t.Country.IsInWarAgainstCountry(Country));
            if (a != null) {
                new Battle(this, a, Province);
            }
        }
    }
}