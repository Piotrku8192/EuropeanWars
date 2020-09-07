﻿using EuropeanWars.Core.Country;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class CountryButton : MonoBehaviour {
        public Image crest;
        public CountryInfo country;
        public DescriptionText description;

        public void SetCountry(CountryInfo country) {
            this.country = country;
            crest.sprite = country.crest;
            description.text = country.name;
        }

        public void OnClick() {
            UIManager.Singleton.CloseAllWindows();
            DiplomacyWindow.Singleton.UpdateWindow(country);
        }
    }
}
