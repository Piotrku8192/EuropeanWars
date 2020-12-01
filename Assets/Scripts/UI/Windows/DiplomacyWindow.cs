using EuropeanWars.Core;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.War;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.UI.Windows {
    public class DiplomacyWindow : MonoBehaviour {
        public static DiplomacyWindow Singleton { get; private set; }

        public GameObject window;

        public Transform countriesListContent;
        public CountryBelt countryBeltPrefab;

        public DipRequestWindow dipRequestWindowPrefab;
        public WarInvitationWindow warInvitationWindowPrefab;
        public CountryInfoWindow countryWindow;

        private Dictionary<CountryInfo, CountryBelt> countries = new Dictionary<CountryInfo, CountryBelt>();

        public void Awake() {
            Singleton = this;
        }

        public void UpdateLanguage() {
            countryWindow.UpdateLanguage();
        }

        public void UpdateWindow(CountryInfo country) {
            UIManager.Singleton.CloseAllWindows();
            window.SetActive(true);

            foreach (var item in GameInfo.countries) {
                if (!countries.ContainsKey(item.Value) && item.Value.nationalProvinces.Count > 0 && item.Key != 0) {
                    CountryBelt belt = Instantiate(countryBeltPrefab, countriesListContent);
                    belt.SetCountry(item.Value);
                    countries.Add(item.Value, belt);
                }
                else if (countries.ContainsKey(item.Value) && item.Value.nationalProvinces.Count == 0) {
                    Destroy(countries[item.Value].gameObject);
                    countries.Remove(item.Value);
                }
            }

            foreach (var item in countries) {
                item.Value.CompareTo(country);
                item.Value.gameObject.SetActive(item.Key != country);
            }

            CountryBelt[] sorted = countries.Values.OrderBy(t => t.country.id).ToArray();
            for (int i = 0; i < sorted.Length; i++) {
                sorted[i].transform.SetSiblingIndex(i);
            }

            countryWindow.UpdateWindow(country);
        }
        public void UpdateWindow() {
            UpdateWindow(countryWindow.country ?? GameInfo.PlayerCountry);
        }

        public DipRequestWindow SpawnRequest(CountryInfo c1, CountryInfo c2, bool isNotification = false) {
            DipRequestWindow go = Instantiate(dipRequestWindowPrefab, UIManager.Singleton.ui.transform);
            go.c1 = c1;
            go.c2 = c2;
            go.Init(isNotification);
            return go;
        }

        public WarInvitationWindow SpawnWarInvitation(WarInfo war, CountryInfo inviter, bool isAttacker) {
            WarInvitationWindow go = Instantiate(warInvitationWindowPrefab, UIManager.Singleton.ui.transform);
            go.Init(war, inviter, isAttacker);
            return go;
        }
    }
}
