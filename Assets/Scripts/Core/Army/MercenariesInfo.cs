using UnityEngine;

namespace EuropeanWars.Core.Army {
    public class MercenariesInfo {
        public int id;
        public string name;
        public Sprite image;
        public int cost;
        public UnitInfo[] units;
        public int[] unitsCount;
    }
}