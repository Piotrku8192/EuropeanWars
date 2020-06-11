using Boo.Lang;
using EuropeanWars.Core;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using System.Collections.Generic;
using UnityEngine;

namespace EuropeanWars.UI.Windows {
    public class DiplomacyWindow : MonoBehaviour {
        public static DiplomacyWindow Singleton { get; private set; }

        public GameObject window;

        public Transform countriesListContent;
        public CountryButton countryButtonPrefab;

        public DipRequestWindow dipRequestWindowPrefab;
        public DiplomacyCountryInfoWindow countryWindow;

        private Dictionary<CountryInfo, CountryButton> countries = new Dictionary<CountryInfo, CountryButton>();

        public void Awake() {
            Singleton = this;
        }

        public void UpdateWindow(CountryInfo country) {
            UIManager.Singleton.CloseAllWindows();
            window.SetActive(true);

            foreach (var item in GameInfo.countries) {
                if (!countries.ContainsKey(item.Value)) {
                    CountryButton button = Instantiate(countryButtonPrefab, countriesListContent);
                    button.SetCountry(item.Value);
                    countries.Add(item.Value, button);
                }
                //TODO: Remove countries without territory.
            }

            countryWindow.UpdateWindow(country);
        }
        public void UpdateWindow() {
            UIManager.Singleton.CloseAllWindows();
            window.SetActive(true);

            foreach (var item in GameInfo.countries) {
                if (!countries.ContainsKey(item.Value) && item.Key > 0) {
                    CountryButton button = Instantiate(countryButtonPrefab, countriesListContent);
                    button.SetCountry(item.Value);
                    countries.Add(item.Value, button);
                }
                //TODO: Remove countries without territory.
            }

            countryWindow.UpdateWindow(GameInfo.PlayerCountry);
        }

        public DipRequestWindow SpawnRequest(DiplomaticRelation relation, bool isNotification = false) {
            DipRequestWindow go = Instantiate(dipRequestWindowPrefab, UIManager.Singleton.ui.transform);
            go.relation = relation;
            go.Init(isNotification);
            return go;
        }
    }
}
