using EuropeanWars.Core.Country;
using EuropeanWars.Core.Pathfinding;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Time;
using EuropeanWars.Network;
using EuropeanWars.UI;
using EuropeanWars.UI.Windows;
using Lidgren.Network;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.Core.Army {
    public class ArmyInfo {
        public static List<ArmyInfo> selectedArmies = new List<ArmyInfo>();
        public readonly int id;

        public Dictionary<UnitInfo, int> units = new Dictionary<UnitInfo, int>();
        public Dictionary<UnitInfo, int> maxUnits = new Dictionary<UnitInfo, int>();

        public int Size => units.Values.Sum();
        public int MaxSize => maxUnits.Values.Sum();
        public int Artilleries => GetArtilleries();
        public int AverageSpeed => units.Sum(t => t.Key.speed * t.Value) / Size;
        public int Maintenance => units.Sum(t => t.Key.maintenance * t.Value);
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
            id = GameInfo.armies.Count;
            GameInfo.armies.Add(GameInfo.armies.Count, this);

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
                    i += item.Value;
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
            UnselectArmy();
            GameInfo.armies.Remove(id);
            Country.armies.Remove(this);
            Province.armies.Remove(this);
            TimeManager.onDayElapsed -= ArmyObject.CountMovement;
            ArmyObject.StopAllCoroutines();
            ArmySpawner.Singleton.DestroyArmy(ArmyObject);
        }

        public void AppendUnit(UnitToRecruit unit) {
            if (Province == unit.province && unit.country == Country) {
                units.Add(unit.unitInfo, unit.count * unit.unitInfo.recruitSize);
                maxUnits.Add(unit.unitInfo, unit.count * unit.unitInfo.recruitSize);
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
            
            //TODO: Add occupation and battle finding

        }
    }
}