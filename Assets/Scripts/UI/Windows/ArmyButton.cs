using EuropeanWars.Core.Army;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class ArmyButton : MonoBehaviour {
        public ArmyInfo army;
        public Text province;
        public Text size;
        
        public void SetArmy(ArmyInfo army) {
            this.army = army;
        }

        public void Update() {
            if (army != null) {
                province.text = army.Province.name;
                size.text = $"{army.Size}/{army.MaxSize}";
                size.color = Color.Lerp(Color.red, Color.green, army.Size / (float)army.MaxSize);
            }
        }
    }
}
