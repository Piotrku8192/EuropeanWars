using EuropeanWars.Core.Diplomacy;
using UnityEngine;

namespace EuropeanWars.UI.Windows {
    public class DiplomacyWindow : MonoBehaviour {
        public static DiplomacyWindow Singleton { get; private set; }

        public GameObject window;
        public DipRequestWindow dipRequestWindowPrefab;
        public DiplomacyCountryInfoWindow countryWindow;

        public void Awake() {
            Singleton = this;
        }

        public void SpawnRequest(DiplomaticRelation relation) {
            DipRequestWindow go = Instantiate(dipRequestWindowPrefab, UIManager.Singleton.ui.transform);
            go.relation = relation;
            go.Init();
        }
    }
}
