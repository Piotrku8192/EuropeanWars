using EuropeanWars.Core.Army;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class ArmyButton : MonoBehaviour {
        public ArmyInfo army;
        public Text province;
        public Text size;
        public Outline selectionOutline;

        public Outline movingOutline;

        public bool isSelected;
        
        public void SetArmy(ArmyInfo army) {
            this.army = army;
        }

        public void SelectArmy() {
            if (army != null) {
                SelectedArmyWindow.Singleton.SelectArmy(army);
            }
        }

        public void SelectMovingArmy() {
            if (army != null) {
                SelectedArmyWindow.Singleton.SelectMovingArmy(army);
            }
        }

        public void Update() {
            if (army != null) {
                province.text = army.Province.name;
                size.text = $"{army.Size}/{army.MaxSize}";
                size.color = Color.Lerp(Color.red, Color.green, army.Size / (float)army.MaxSize);

                movingOutline.enabled = army == SelectedArmyWindow.Singleton.MovingArmy;
                if (SelectedArmyWindow.Singleton.SelectedArmy != null) {
                    movingOutline.gameObject.SetActive(army != SelectedArmyWindow.Singleton.SelectedArmy 
                        && army != SelectedArmyWindow.Singleton.MovingArmy 
                        && army.Province == SelectedArmyWindow.Singleton.SelectedArmy.Province);
                }
            }

            selectionOutline.enabled = isSelected;
        }
    }
}
