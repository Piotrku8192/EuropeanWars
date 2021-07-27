using EuropeanWars.Core.Data;
using EuropeanWars.Core.Language;
using UnityEngine;

namespace EuropeanWars.Core.Army {
    public class MercenariesInfo {
        private readonly MercenariesData data;
        public int id;
        public string name;
        public Sprite image;
        public int cost;
        public UnitInfo[] units;
        public int[] unitsCount;

        public MercenariesInfo(MercenariesData data) {
            this.data = data;
            id = data.id;
            cost = data.cost;
            unitsCount = data.unitsCount;
        }

        public void Initialize() {
            image = GameInfo.gfx[$"Mercenaries-{id}"];
            
            units = new UnitInfo[data.units.Length];
            for (int i = 0; i < data.units.Length; i++) {
                units[i] = GameInfo.units[data.units[i]];
            }
        }

        public void UpdateLanguage() {
            name = LanguageDictionary.language[$"Mercenaries-{id}"];
        }
    }
}