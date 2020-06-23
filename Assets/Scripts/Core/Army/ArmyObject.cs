using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.Core.Army {
    public class ArmyObject : MonoBehaviour {
        public ArmyInfo army;
        public Image crest;
        public Text size;
        public Image sizeBackground;

        public void Initialize(ArmyInfo army) {
            this.army = army;
            crest.sprite = army.country.crest;
            transform.position = new Vector3(army.province.x, army.province.y, 0);
        }

        public void Update() {
            size.text = $"{army.Size * 0.001f}k";
        }
    }
}
