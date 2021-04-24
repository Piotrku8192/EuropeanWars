using EuropeanWars.Core.Country;
using EuropeanWars.Core.Province;

namespace EuropeanWars.Core.Army {
    public class UnitToRecruit {
        public UnitInfo unitInfo;
        public ProvinceInfo province;
        public CountryInfo country;
        public int count;
        public int days;

        public void RecruitUnit() {
            new ArmyInfo(this);
        }

        public bool Equals(UnitToRecruit unit) {
            return unitInfo == unit.unitInfo && province == unit.province && country == unit.country && days == unit.days;
        }
    }
}
