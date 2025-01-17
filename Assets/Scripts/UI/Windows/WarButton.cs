﻿using EuropeanWars.Core.War;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class WarButton : MonoBehaviour {
        public Image enemiesMajorCrest;
        public Text warScore;
        public WarCountryInfo Country { get; private set; }

        public void Init(WarCountryInfo warCountryInfo) {
            Country = warCountryInfo;
            enemiesMajorCrest.sprite = Country.party.Enemies.major.country.Crest;
        }

        public void OnClick() {
            WarWindow.Singleton.SetWar(Country.party.war);
        }

        public void Update() {
            if (Country != null) {
                warScore.text = (Country.IsMajor ? Country.party.PercentWarScore : Country.PercentWarScore) + "%";
                warScore.color = Country.IsMajor ? Country.party.PercentWarScoreColor : Country.PercentWarScoreColor;
            }
        }
    }
}
