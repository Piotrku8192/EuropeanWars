using EuropeanWars.Core.Country;
using EuropeanWars.Core.War;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class CountryBelt : MonoBehaviour {
        public CountryButton button;
        public DescriptionText war;
        public GameObject vassal;
        public DescriptionText truce;
        public GameObject[] relations;
        public Text points;
        public CountryInfo country;

        public int activeRelations;

        public void SetCountry(CountryInfo country) {
            this.country = country;
            button.SetCountry(country);
        }

        /// <summary>
        /// Returns number of relations that links both countries
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public int CompareTo(CountryInfo country) {
            if (this.country == country) {
                war.gameObject.SetActive(false);
                vassal.gameObject.SetActive(false);
                for (int i = 0; i < relations.Length; i++) {
                    relations[i].SetActive(false);
                }
                points.text = "-";
                points.color = Color.gray;
                return 0;
            }
            else {
                int result = 0;

                bool w = country.IsInWarAgainstCountry(this.country);
                war.gameObject.SetActive(w);
                if (w) {
                    WarCountryInfo wci = country.wars[country.GetWarAgainstCountry(this.country)];
                    Color32 c = wci.PercentWarScoreColor;
                    war.text = "<color=#" + c.r.ToString("X2") + c.g.ToString("X2") + c.b.ToString("X2") + ">" + wci.PercentWarScore.ToString() + "%</color>";
                    result++;
                }

                vassal.SetActive(false); //TODO: Change it to country.vassals.Contains(this.country);
                truce.gameObject.SetActive(country.relations[this.country].truceInMonths > 0);
                truce.text = $"{country.relations[this.country].truceInMonths} months left";

                for (int i = 0; i < relations.Length; i++) {
                    if (country.relations[this.country].relations[i]) {
                        relations[i].SetActive(true);
                        result++;
                    }
                    else {
                        relations[i].SetActive(false);
                    }
                }

                int p = country.relations[this.country].Points;
                points.text = p.ToString();
                points.color = Color.Lerp(Color.red, Color.green, (p + 100) / 200.0f);

                return result;
            }
        }
    }
}
