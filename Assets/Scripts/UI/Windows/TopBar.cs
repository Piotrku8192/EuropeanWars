using EuropeanWars.Core;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class TopBar : MonoBehaviour {
        public Text gold;
        public Text manpower;
        public Text prestige;

        public void Update() {
            if (GameInfo.gameStarted) {
                gold.text = GameInfo.PlayerCountry.gold.ToString();
                gold.color = GameInfo.PlayerCountry.gold < 0 ? Color.red : Color.white;
                manpower.text = GameInfo.PlayerCountry.manpower.ToString();
                prestige.text = GameInfo.PlayerCountry.prestige.ToString();
            }
        }
    }
}
