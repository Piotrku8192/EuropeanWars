using System;

namespace EuropeanWars.Core.Data {
    [Serializable]
    public class UnitData {
        public int id;
        public string name;
        public string image;
        public int type;
        public int recruitBuilding;
        public int recruitSize;
        public int recruitCost;
        public int recruitDays;
        public int attack;
        public int health;
        public int attackCooldown;
        public int speed;
    }
}
