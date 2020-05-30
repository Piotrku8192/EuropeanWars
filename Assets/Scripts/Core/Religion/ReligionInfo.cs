using EuropeanWars.Core.Data;
using EuropeanWars.Core.Language;
using UnityEngine;

namespace EuropeanWars.Core.Religion {
    public class ReligionInfo {
        public int id;
        public string name;
        public Color color;
        public Sprite image;

        private ReligionData data;

        public ReligionInfo(ReligionData data) {
            this.data = data;
            id = data.id;
            color = DataConverter.ToColor(int.Parse(data.color, System.Globalization.NumberStyles.HexNumber));
        }

        public void Initialize() {
            image = GameInfo.gfx["Religion-" + id];
            UpdateLanguage();
        }

        public void UpdateLanguage() {
            if (id > 0) {
                name = LanguageDictionary.language["ReligionName-" + id];
            }
        }
    }
}
