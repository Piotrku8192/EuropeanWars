using EuropeanWars.Core.Data;
using EuropeanWars.Core.Language;
using UnityEngine;

namespace EuropeanWars.Core.Culture {
    public class CultureInfo {
        public int id;
        public string name;
        public Color color;

        private CultureData data;

        public CultureInfo(CultureData data) {
            this.data = data;
            id = data.id;
            color = DataConverter.ToColor(int.Parse(data.color, System.Globalization.NumberStyles.HexNumber));
        }

        public void Initialize() {
        }

        public void UpdateLanguage() {
            name = LanguageDictionary.language["CultureName-" + id];
        }
    }
}
