using EuropeanWars.Core.Country;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Time;
using EuropeanWars.GameMap;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EuropeanWars.Core.Pathfinding {
    public class ArmyInfo {
        public static List<ArmyInfo> selectedArmies = new List<ArmyInfo>();

        public Dictionary<UnitInfo, int> units = new Dictionary<UnitInfo, int>();
        public Dictionary<UnitInfo, int> maxUnits = new Dictionary<UnitInfo, int>();

        public int Size => units.Values.Sum();
        public int AverageSpeed => units.Sum(t => t.Key.speed * t.Value) / Size;
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

            ArmyObject = ArmySpawner.Singleton.SpawnAndInitializeArmy(this);
            TimeManager.onDayElapsed += ArmyObject.CountMovement;
        }

        public void Delete() {
            Country.armies.Remove(this);
            Province.armies.Remove(this);
            TimeManager.onDayElapsed -= ArmyObject.CountMovement;
            ArmySpawner.Singleton.DestroyArmy(ArmyObject);
        }

        public void AppendUnit(UnitToRecruit unit) {
            if (Province == unit.province && unit.country == Country) {
                units.Add(unit.unitInfo, unit.count * unit.unitInfo.recruitSize);
                maxUnits.Add(unit.unitInfo, unit.count * unit.unitInfo.recruitSize);
            }
        }

        public void SelectArmy(bool unselectOthers = true) {
            if (Country == GameInfo.PlayerCountry) {
                if (unselectOthers) {
                    foreach (var item in selectedArmies) {
                        item.UnselectArmy();
                    }
                }
                selectedArmies.Add(this);
                IsSelected = true;
                ArmyObject.selectionOutline.gameObject.SetActive(true);
            }
        }

        public void UnselectArmy() {
            selectedArmies.Remove(this);
            IsSelected = false;
            ArmyObject.selectionOutline.gameObject.SetActive(false);
        }

        public void FindRoute(ProvinceInfo target) {
            LandArmyPathfinder pathfinder = new LandArmyPathfinder(this);
            ProvinceInfo[] r = pathfinder.FindPath(target);
            if (r != null) {
                route.Clear();
                foreach (var item in r) {
                    route.Enqueue(item);
                }
                ArmyObject.DrawRoute(route.Last(), route.ToArray());
            }
        }

        public void OnArmyMove(ProvinceInfo newProvince) {
            Province.armies.Remove(this);
            newProvince.armies.Add(this);

            if (Country == GameInfo.PlayerCountry || GameInfo.PlayerCountry.alliances.ContainsKey(Country)) {
                Province.RefreshFogOfWarInRegion();
                newProvince.SetFogOfWar(false);
            }

            Province = newProvince;
            BlackStatus = Country != Province.Country && !Country.militaryAccesses.ContainsKey(Province.Country); //TODO: Add this: && !country.HasWarWith(province.Country);

            //TODO: Add occupation and battle finding

        }
    }
}