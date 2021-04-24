using EuropeanWars.Core.Army;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Culture;
using EuropeanWars.Core.Language;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Religion;
using System;
using System.Collections.Generic;

namespace EuropeanWars.Core.Data {
    [Serializable]
    public class GameData {
        public Dictionary<string, byte[]> gfx = new Dictionary<string, byte[]>();
        public List<Dictionary<string, string>> languages = new List<Dictionary<string, string>>();
        public ProvinceData[] provinces;
        public CountryData[] countries;
        public ReligionData[] religions;
        public CultureData[] cultures;
        public BuildingData[] buildings;
        public UnitData[] units;
        public ArmyData[] armies;
        public string map;

        public void FillGameInfo() {
            LanguageDictionary.languages = languages;
            LanguageDictionary.language = languages[0];
            int i = 0;

            foreach (var item in buildings) {
                GameInfo.buildings.Add(item.id, new BuildingInfo(item));
            }

            foreach (var item in religions) {
                GameInfo.religions.Add(item.id, new ReligionInfo(item));
            }

            foreach (var item in cultures) {
                GameInfo.cultures.Add(item.id, new CultureInfo(item));
            }

            foreach (var item in countries) {
                GameInfo.countries.Add(item.id, new CountryInfo(item));
            }

            foreach (var item in provinces) {
                item.id = i;
                GameInfo.provinces.Add(item.id, new ProvinceInfo(item));
                i++;
            }

            foreach (var item in units) {
                GameInfo.units.Add(item.id, new UnitInfo(item));
            }
        }
    }
}
