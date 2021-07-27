using EuropeanWars.Core.Army;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class MercenariesButton : MonoBehaviour {
        public Image image;
        public MercenariesInfo mercenaries;
        
        public void SetMercenaries(MercenariesInfo mercenaries) {
            this.mercenaries = mercenaries;
            image.sprite = mercenaries.image;
        }

        public void OnClick() {
            ArmyWindow.Singleton.recrutationWindow.SelectMercenaries(mercenaries);
        }
    }
}