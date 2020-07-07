using Boo.Lang;
using EuropeanWars.Core;
using EuropeanWars.Core.War;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class DeclareWarWindow : MonoBehaviour {
        public static DeclareWarWindow Singleton { get; private set; }
        public Transform warReasonsContent;
        public Button declareButton;
        public WarReasonButton warReasonButtonPrefab;
        public List<WarReasonButton> reasons = new List<WarReasonButton>();
        public WarReasonButton selectedReason;

        public void Awake() {
            Singleton = this;
        }

        public void OnEnable() {
            UpdateWindow();
        }

        public void Update() {
            declareButton.interactable = selectedReason != null;
        }

        public void UpdateWindow() {
            WarReasonFactory factory = new WarReasonFactory(GameInfo.PlayerCountry, DiplomacyWindow.Singleton.countryWindow.country);
            WarReason[] r = factory.GetReasons();
            for (int i = 0; i < r.Length; i++) {
                WarReasonButton go = Instantiate(warReasonButtonPrefab, warReasonsContent);
                go.declareWarWindow = this;
                go.warReason = r[i];
                go.warReasonId = i;
                go.reasonName.text = r[i].Name;
                reasons.Add(go);
            }
        }

        public void ResetAndDisable() {
            selectedReason = null;
            foreach (var item in reasons) {
                Destroy(item.gameObject);
            }
            reasons.Clear();
            gameObject.SetActive(false);
        }
    }
}
