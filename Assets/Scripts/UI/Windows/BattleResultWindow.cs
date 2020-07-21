using EuropeanWars.Core.Army;
using EuropeanWars.Core.Province;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class BattleResultWindow : MonoBehaviour {
        public ArmyInfo winner;
        public ArmyInfo loser;
        public ProvinceInfo province;
        public int winnerKilled;
        public int loserKilled;

        public Text provinceName;
        public Image winnerCrest;
        public Image loserCrest;
        public Image winnerImage;
        public Image loserImage;
        public Text winnerKilledText;
        public Text loserKilledText;

        public void Init(ArmyGroup winner, ArmyGroup loser, ProvinceInfo province, int winnerKilled, int loserKilled) {
            this.winner = winner.Armies.First();
            this.loser = loser.Armies.First();
            this.province = province;
            this.winnerKilled = winnerKilled;
            this.loserKilled = loserKilled;

            provinceName.text = province.name;
            winnerCrest.sprite = winner.Armies.First().Country.crest;
            loserCrest.sprite = loser.Armies.First().Country.crest;
            winnerImage.sprite = winner.Armies.First().maxUnits.OrderBy(t => t.Value).Last().Key.image;
            loserImage.sprite = loser.Armies.First().maxUnits.OrderBy(t => t.Value).Last().Key.image;
            winnerKilledText.text = winnerKilled.ToString();
            loserKilledText.text = loserKilled.ToString();
        }

        public void OnClick() {
            Destroy(gameObject);
        }
    }
}
