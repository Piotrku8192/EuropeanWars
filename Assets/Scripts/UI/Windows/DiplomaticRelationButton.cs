using EuropeanWars.Core.Diplomacy;
using UnityEngine;

namespace EuropeanWars.UI.Windows {
    public class DiplomaticRelationButton : MonoBehaviour {
        public DiplomaticRelation action;
        public bool targetState;

        public void OnClick() {
            DiplomacyWindow.Singleton.countryWindow.TryChangeRelationState(action, targetState);
        }
    }
}
