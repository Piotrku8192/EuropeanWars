using EuropeanWars.Core.Building;
using EuropeanWars.Core.Data;
using EuropeanWars.Core.Language;
using UnityEngine;

namespace EuropeanWars.Core.Army {
    public class UnitInfo {
        private readonly UnitData data;

        public int id;
        public string name;
        public Sprite image;
        public UnitType type;
        public BuildingInfo recruitBuilding;
        public int recruitSize;
        public int recruitCost;
        public int recruitDays;
        public int attack;
        public int health;
        public int attackCooldown;
        public int speed;
        public float maintenance;

        public UnitInfo(UnitData data) {
            this.data = data;
            id = data.id;
            type = (UnitType)data.type;
            recruitSize = data.recruitSize;
            recruitCost = data.recruitCost;
            recruitDays = data.recruitDays;
            attack = data.attack;
            health = data.health;
            attackCooldown = data.attackCooldown;
            speed = data.speed;
            maintenance = data.maintenance;
        }

        public void Initialize() {
            image = GameInfo.gfx[data.image];
            recruitBuilding = GameInfo.buildings[data.recruitBuilding];
        }

        public void UpdateLanguage() {
            name = LanguageDictionary.language["UnitName-" + id];
        }
    }
}
