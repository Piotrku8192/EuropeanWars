using EuropeanWars.Core.Army;
using EuropeanWars.Core.Province;
using UnityEngine;

namespace EuropeanWars.UI.Windows {
    public class BattleResultWindowSpawner : MonoBehaviour {
        public static BattleResultWindowSpawner Singleton { get; private set; }

        public BattleResultWindow prefab;

        public void Awake() {
            Singleton = this;
        }

        public void Spawn(ArmyInfo winner, ArmyInfo loser, ProvinceInfo province, int winnerKilled, int loserKilled) {
            BattleResultWindow w = Instantiate(prefab, UIManager.Singleton.ui.transform);
            w.Init(winner, loser, province, winnerKilled, loserKilled);
        }
    }
}
