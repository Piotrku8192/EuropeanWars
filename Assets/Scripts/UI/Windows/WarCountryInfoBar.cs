﻿using EuropeanWars.Core;
using EuropeanWars.Core.War;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class WarCountryInfoBar : MonoBehaviour{
        public WarCountryInfo country;
        public CountryButton countryButton;
        public Text countryName;
        public Text warScore;
        public Text enemyOccupatedProvinces;
        public Text localOccupatedProvinces;
        public Text killedEnemies;
        public Text killedLocal;
        public GameObject peaceButton;

        public void SetCountry(WarCountryInfo country) {
            this.country = country;
            countryButton.SetCountry(country.country);
            countryName.text = country.country.name;
            peaceButton.SetActive(
                (country.party.Enemies.ContainsCountry(GameInfo.PlayerCountry) && country.party.major == country)
                || (country.party.Enemies.major.country == GameInfo.PlayerCountry));
        }

        public void Update() {
            if (country != null) {
                warScore.text = Mathf.FloorToInt(country.PercentWarScore * 100) + "%";
                warScore.color = country.PercentWarScoreColor;
                enemyOccupatedProvinces.text = country.enemyOccupatedProvinces.Count.ToString();
                localOccupatedProvinces.text = country.localOccupatedProvinces.Count.ToString();
                killedEnemies.text = country.killedEnemies.ToString();
                killedLocal.text = country.killedLocal.ToString();
            }
        }

        public void Peace() {

        }
    }
}