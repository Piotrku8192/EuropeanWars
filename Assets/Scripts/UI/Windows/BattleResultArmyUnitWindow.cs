using EuropeanWars.Core.Army;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Windows {
    public class BattleResultArmyUnitWindow : MonoBehaviour {
        public UnitInfo unit;
        public Image unitImage;
        public Text size;
        public Text sizeChange;

        public void SetUnit(UnitInfo unit, int size, int maxSize, int sizeChange) {
            this.unit = unit;
            unitImage.sprite = unit.image;
            this.size.text = $"{size}/{maxSize}";
            this.size.color = Color.Lerp(Color.red, Color.green, (float)size / maxSize);
            this.sizeChange.text = sizeChange.ToString();
        }
    }
}
