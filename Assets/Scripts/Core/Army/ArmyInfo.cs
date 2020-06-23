using EuropeanWars.Core.Country;
using EuropeanWars.Core.Province;
using System.Collections.Generic;
using System.Linq;

namespace EuropeanWars.Core.Army {
    public class ArmyInfo {
        public Dictionary<UnitInfo, int> units = new Dictionary<UnitInfo, int>();
        public Dictionary<UnitInfo, int> maxUnits = new Dictionary<UnitInfo, int>();

        public int Size => units.Values.Sum();

        public CountryInfo country;
        public ProvinceInfo province;
        public ArmyObject armyObject;

        public ArmyInfo(UnitToRecruit unit) {
            units.Add(unit.unitInfo, unit.count * unit.unitInfo.recruitSize);
            maxUnits.Add(unit.unitInfo, unit.count * unit.unitInfo.recruitSize);
            country = unit.country;
            province = unit.province;
            country.armies.Add(this);
            province.armies.Add(this);

            ArmySpawner.Singleton.SpawnAndInitializeArmy(this);
        }

        public void Delete() {
            country.armies.Remove(this);
            province.armies.Remove(this);
            ArmySpawner.Singleton.DestroyArmy(armyObject);
        }

        public void AppendUnit(UnitToRecruit unit) {
            if (province == unit.province && unit.country == country) {
                units.Add(unit.unitInfo, unit.count * unit.unitInfo.recruitSize);
                maxUnits.Add(unit.unitInfo, unit.count * unit.unitInfo.recruitSize);
            }
        }
    }
}