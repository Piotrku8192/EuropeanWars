using EuropeanWars.Core.Army;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI {
    public class UnitButton : MonoBehaviour {
        public UnitInfo unit;
        public Image image;

        public void SetUnit(UnitInfo unit) {
            this.unit = unit;
            image.sprite = unit.image;
        }

        public void OnClick() {
            ArmyWindow.Singleton.recrutationWindow.SelectUnit(unit);
        }
    }
}
