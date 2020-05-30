using EuropeanWars.Core.Country;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class CountryButton : MonoBehaviour {
        public Image crest;
        public CountryInfo country;

        public void SetCountry(CountryInfo country) {
            this.country = country;
            crest.sprite = country.crest;
        }

        public void OnClick() {
            UIManager.Singleton.CloseAllWindows();
            DiplomacyWindow.Singleton.window.SetActive(true);
            DiplomacyWindow.Singleton.countryWindow.UpdateWindow(country);
        }
    }
}
