using UnityEngine;

namespace EuropeanWars.Core.Data {
    public struct BuildingD {
        public string name;
        public int id;
        public string image;
        public int cost;
        public int country;
        public int terrain;
        public int minTaxation;
        public int level;
        public bool canBuild;
        public int incomeModifier;
        public float incomeModifierModifier;
        public int defenceModifier;

        public static BuildingD BuildingToBuildingData(BuildingData building) {
            return new BuildingD() {
                id = building.id,
                image = building.id.ToString(),
                cost = building.cost,
                terrain = building.terrain,
                minTaxation = building.minTaxation,
                incomeModifier = building.incomeModifier,
                defenceModifier = building.defenceModifier
            };
        }
        public static BuildingData BuildingDataToBuilding(BuildingD buildingData) {
            BuildingData result = new BuildingData() {
                id = buildingData.id,
                cost = buildingData.cost,
                terrain = buildingData.terrain,
                minTaxation = buildingData.minTaxation,
                incomeModifier = buildingData.incomeModifier,
                defenceModifier = buildingData.defenceModifier
            };

            return result;
        }
    }
    public struct CultureD {
        public string name;
        public int id;
        public Color32 color;

        public static CultureD CultureToCultureData(CultureData culture) {
            return new CultureD() {
                id = culture.id,
        };
        }
        public static CultureData CultureDataToCulture(CultureD cultureData) {
            return new CultureData() {
                id = cultureData.id,
                color = cultureData.color.r.ToString("X2") + cultureData.color.g.ToString("X2") + cultureData.color.b.ToString("X2")
            };
        }
    }
    public struct ReligionD {
        public string name;
        public int id;
        public string image;
        public Color32 color;

        public static ReligionD RelitionToReligionData(ReligionData religion) {
            return new ReligionD() {
                id = religion.id,
                image = religion.id.ToString(),
            };
        }
        public static ReligionData RelitionDataToReligion(ReligionD religionData) {
            ReligionData result = new ReligionData() {
                id = religionData.id,
                color = religionData.color.r.ToString("X2") + religionData.color.g.ToString("X2") + religionData.color.b.ToString("X2")
            };

            return result;
        }
    }
}
