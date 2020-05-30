using EuropeanWars.Core.Country;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class DiplomacyCountryInfoWindow : MonoBehaviour {
        public Image crest;
        public Image religion;
        public Image king;
        public Text countryName;

        public CountryInfo country;

        public void UpdateWindow(CountryInfo country) {
            this.country = country;
            crest.sprite = country.crest;
            religion.sprite = country.religion.image;
            //king.sprite = country.king.image; TODO: uncomment this.
            countryName.text = country.name;
        }
    }
}
