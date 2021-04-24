using System;

namespace EuropeanWars.Core.Data {
    [Serializable]
    public class BuildingData {
        public int id;
        public int cost;
        public int terrain;
        public int minTaxation;
        public int incomeModifier;
        public int foodModifier;
        public int defenceModifier;
    }
}
