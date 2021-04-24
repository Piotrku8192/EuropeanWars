using EuropeanWars.Core.Language;
using System;
using UnityEngine;

namespace EuropeanWars.UI {
    public class TranslatedDescriptionsLibrary : MonoBehaviour {
        public static TranslatedDescriptionsLibrary Singleton { get; private set; }

        [Serializable]
        public class TranslatedDescription {
            public string translateKey;
            public DescriptionText[] texts;
        }

        public TranslatedDescription[] descriptions;

        public void Awake() {
            Singleton = this;
        }

        public void UpdateLanguage() {
            foreach (var item in descriptions) {
                for (int i = 0; i < item.texts.Length; i++) {
                    if (LanguageDictionary.language.ContainsKey(item.translateKey)) {
                        item.texts[i].text = LanguageDictionary.language[item.translateKey];
                    }
                    else {
                        item.texts[i].text = "NO TRANSLATION";
                    }
                }
            }
        }
    }
}
