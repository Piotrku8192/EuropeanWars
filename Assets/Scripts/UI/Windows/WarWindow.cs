using EuropeanWars.Core.War;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class WarWindow : MonoBehaviour {
        public static WarWindow Singleton { get; private set; }

        public WarInfo war;

        public GameObject windowObject;

        public Text startDate;
        public CountryButton attackersMajor;
        public CountryButton defendersMajor;
        public Text warReason;
        public Text attackersWarScore;
        public Text defendersWarScore;

        [Space(15)]
        public WarCountryInfoBar warCountryInfoBarPrefab;
        public Transform attackersContent;
        public Transform defendersContent;

        private List<WarCountryInfoBar> attackers = new List<WarCountryInfoBar>();
        private List<WarCountryInfoBar> defenders = new List<WarCountryInfoBar>();

        public void Awake() {
            Singleton = this;
        }

        public void SetWar(WarInfo war) {
            UIManager.Singleton.CloseAllWindows();
            this.war = war;
            if (war == null) {
                windowObject.SetActive(false);
                return;
            }

            windowObject.SetActive(true);
            startDate.text = $"{(war.startDay < 10 ? "0" : "")}{war.startDay}." +
                $"{(war.startMonth < 10 ? "0" : "")}{war.startMonth}." +
                $"{(war.startYear < 10 ? "0" : "")}{war.startYear}";
            attackersMajor.SetCountry(war.attackers.major.country);
            defendersMajor.SetCountry(war.defenders.major.country);
            warReason.text = war.warReason.Name;

            foreach (var item in attackers) {
                Destroy(item.gameObject);
            }
            foreach (var item in defenders) {
                Destroy(item.gameObject);
            }
            attackers.Clear();
            defenders.Clear();

            foreach (var item in war.attackers.countries) {
                WarCountryInfoBar b = Instantiate(warCountryInfoBarPrefab, attackersContent);
                b.SetCountry(item.Value);
                attackers.Add(b);
            }

            foreach (var item in war.defenders.countries) {
                WarCountryInfoBar b = Instantiate(warCountryInfoBarPrefab, defendersContent);
                b.SetCountry(item.Value);
                defenders.Add(b);
            }
        }

        public void Update() {
            if (war != null) {
                attackersWarScore.text = war.attackers.PercentWarScore + "%";
                attackersWarScore.color = war.attackers.PercentWarScoreColor;
                defendersWarScore.text = war.defenders.PercentWarScore + "%";
                defendersWarScore.color = war.defenders.PercentWarScoreColor;
            }
        }
    }
}
