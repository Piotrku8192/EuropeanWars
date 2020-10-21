using EuropeanWars.Core;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class TradeWindow : MonoBehaviour
    {
        public static TradeWindow Singleton { get; private set; }

        public GameObject window;
        public Text title;
        public Text modificators;

        public Image[] countries;
        public DescriptionText statText;

        private int totalCities;
        private List<(int, CountryInfo)> cs = new List<(int, CountryInfo)>();

        public void Awake() {
            Singleton = this;
        }

        public void Update() {
            if (window.activeInHierarchy) {
                UpdateRank();
                UpdateOnMouseOver();
            }
        }

        public void UpdateLanguage() {
            title.text = LanguageDictionary.language["Description-trade"];
            modificators.text = LanguageDictionary.language["TradeModificators"];
        }

        public void UpdateWindow() {
            UIManager.Singleton.CloseAllWindows();
            window.SetActive(true);
        }

        public void UpdateRank() {
            cs.Clear();
            totalCities = GameInfo.provinces.Where(t => t.Value.isTradeCity).Count();
            foreach (var item in GameInfo.countries.Values) {
                cs.Add((item.provinces.Where(x => x.isTradeCity).Count(), item));
            }

            cs = cs.OrderBy(t => -t.Item1).ToList();
            //cs.Reverse();
            float f = 0;

            for (int i = 0; i < countries.Length; i++) {
                if (cs.Count > i) {
                    float percent = cs[i].Item1 / (float)totalCities;
                    countries[i].color = cs[i].Item2.color;
                    countries[i].fillAmount = percent + f;
                    f += percent;
                }
                else {
                    countries[i].color = Color.clear;
                }
            }
        }

        public void UpdateOnMouseOver() {
            if (cs.Count == 0)
                return;

            Vector2 mouse = new Vector2();
            mouse.x = Input.mousePosition.x - statText.transform.position.x;
            mouse.y = Input.mousePosition.y - statText.transform.position.y;
            mouse.Normalize();
            float angle = ((Mathf.Atan2(mouse.y, -mouse.x) / Mathf.PI) + 1) / 2;

            float last = 0;
            bool isOther = true;

            for (int i = 0; i < countries.Length; i++) {
                float percent = cs[i].Item1 / (float)totalCities;

                if (angle > last && angle <= percent + last) {
                    statText.text = $"{cs[i].Item2.name} {Math.Round(percent * 100f, 1)}%";
                    isOther = false;
                    last += percent;
                    break;
                }

                last += percent;
            }

            if (isOther)
                statText.text = $"{LanguageDictionary.language["Others"]} {Math.Round((1 - last) * 100f, 1)}%";
        }
    }
}
