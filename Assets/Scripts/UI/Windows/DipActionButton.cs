using UnityEngine;

namespace EuropeanWars.UI.Windows {
    public class DipActionButton : MonoBehaviour
    {
        public DiplomacyAction action;

        public void OnClick() {
            DiplomacyWindow.Singleton.countryWindow.PlayAction(action);
        }
    }
}
