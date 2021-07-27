using System;

namespace EuropeanWars.Core.Data {
    [Serializable]
    public class MercenariesData {
        public int id;
        public int cost;
        public int[] units;
        public int[] unitsCount;
    }
}