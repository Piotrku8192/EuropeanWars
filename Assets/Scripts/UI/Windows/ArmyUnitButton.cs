using EuropeanWars.Core.Army;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class ArmyUnitButton : MonoBehaviour {
        public UnitInfo unit;
        public ArmyInfo army;
        public Image image;
        public Text unitName;
        public Text size;
        public GameObject moveUnitButtons;

        public void SetUnit(UnitInfo unit, ArmyInfo army) {
            this.unit = unit;
            this.army = army;
            image.sprite = unit.image;
            unitName.text = unit.name;

            if (army == SelectedArmyWindow.Singleton.MovingArmy) {
                moveUnitButtons.SetActive(true);
            }
        }

        public void Update() {
            if (army != null && unit != null) {
                size.text = $"{army.units[unit]}/{army.maxUnits[unit]}";
                size.color = Color.Lerp(Color.red, Color.green, army.units[unit] / (float)army.maxUnits[unit]);
            }
        }

        public void MoveUnitToSecondArmy(int count) {
            if (army == SelectedArmyWindow.Singleton.SelectedArmy 
                && SelectedArmyWindow.Singleton.MovingArmy != null) {
                army.MoveUnitToOtherArmyRequest(unit, SelectedArmyWindow.Singleton.MovingArmy, unit.recruitSize * count);
            }
            else if (army == SelectedArmyWindow.Singleton.MovingArmy 
                && SelectedArmyWindow.Singleton.SelectedArmy != null) {
                army.MoveUnitToOtherArmyRequest(unit, SelectedArmyWindow.Singleton.SelectedArmy, unit.recruitSize * count);
            }
        }
    }
}
