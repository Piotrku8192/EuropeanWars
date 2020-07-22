using Boo.Lang;
using EuropeanWars.Core;
using EuropeanWars.Core.Army;
using EuropeanWars.Core.Province;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class BattleResultWindow : MonoBehaviour {
        public BattleArmyGroup winner;
        public BattleArmyGroup loser;
        public ProvinceInfo province;
        public int winnerKilled;
        public int loserKilled;

        public Text title;
        public Image titleBg;
        public Text provinceName;
        public Text winnerKilledText;
        public Text loserKilledText;

        public BattleResultArmyWindow armyWindowPrefab;

        public Transform winnerArmiesContent;
        public List<BattleResultArmyWindow> winnerBattleResultArmyWindows = new List<BattleResultArmyWindow>();
        public Transform loserArmiesContent;
        public List<BattleResultArmyWindow> loserBattleResultArmyWindows = new List<BattleResultArmyWindow>();

        public void Init(BattleArmyGroup winner, BattleArmyGroup loser, ProvinceInfo province, int winnerKilled, int loserKilled) {
            this.winner = winner;
            this.loser = loser;
            this.province = province;
            this.winnerKilled = winnerKilled;
            this.loserKilled = loserKilled;

            provinceName.text = province.name;
            winnerKilledText.text = winnerKilled.ToString();
            loserKilledText.text = loserKilled.ToString();

            foreach (var item in winner.sizeChange) {
                BattleResultArmyWindow a = Instantiate(armyWindowPrefab, winnerArmiesContent);
                a.SetArmy(item.Key, item.Value);
                winnerBattleResultArmyWindows.Add(a);
            }

            foreach (var item in loser.sizeChange) {
                BattleResultArmyWindow a = Instantiate(armyWindowPrefab, loserArmiesContent);
                a.SetArmy(item.Key, item.Value);
                loserBattleResultArmyWindows.Add(a);
            }

            if (winner.Armies.Any(t => t.Country == GameInfo.PlayerCountry)) {
                title.text = "Zwycięstwo!"; //TODO: Add translation
                titleBg.color = new Color(0.21f, 0.6f, 0.0f, 1.0f);
            }
            else {
                title.text = "Porażka!"; //TODO: Add translation
                titleBg.color = new Color(0.6f, 0.07f, 0.0f, 1.0f);
            }
        }

        public void OnClick() {
            Destroy(gameObject);
        }
    }
}
