using EuropeanWars.Core.Country;
using EuropeanWars.Core.Province;

namespace EuropeanWars.Core.Army {
    public class Mercenaries {
        public readonly MercenariesInfo info;
        public readonly ArmyInfo army;

        public Mercenaries(MercenariesInfo info, CountryInfo country, ProvinceInfo province) {
            this.info = info;
            army = new ArmyInfo(province, country, info.units[0], info.unitsCount[0] * info.units[0].recruitSize, info.unitsCount[0] * info.units[0].recruitSize);
            
            for (int i = 1; i < info.units.Length; i++) {
                army.AddUnit(info.units[i], info.unitsCount[i] * info.units[i].recruitSize, info.unitsCount[i] * info.units[i].recruitSize);
            }
            army.maintenanceModifier = 3;

            country.gold -= info.cost;
            country.mercenaries.Add(this);
        }
    }
}