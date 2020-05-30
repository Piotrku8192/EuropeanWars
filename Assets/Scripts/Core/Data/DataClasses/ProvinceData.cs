using System;

namespace EuropeanWars.Core.Data {
    [Serializable]
    public class ProvinceData {
        public int id;
        public string color;
        public int x, y;
        public bool isLand;

        public int taxation;
        public bool isInteractive;
        public bool isActive;

        public int[] neighbours;
        public int country;
        public int religion;
        public int[] religionFollowers;
        public int culture;
        public int defense;
        public int garnison;
        public bool isTradeCity;
        public bool isTradeRoute;

        public int[] buildings = new int[10];
    }
}
