using System;

namespace EuropeanWars.Core.Data {
    [Serializable]
    public class GameData {
        public ProvinceData[] provinces;
        public CountryData[] countries;
        public ReligionData[] religions;
        public CultureData[] cultures;
        public BuildingData[] buildings;
        public UnitData[] units;
    }
}
