using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI {
    [RequireComponent(typeof(Button), typeof(Image))]
    public class TabElement : MonoBehaviour {
        public Color selectedColor;
        public Color unselectedColor;

        [HideInInspector]
        public Button button;
        [HideInInspector]
        public Image image;

        public void Start() {
            button = GetComponent<Button>();
            image = GetComponent<Image>();
        }

        public void Select() {
            image.color = selectedColor;
        }

        public void Unselect() {
            image.color = unselectedColor;
        }
    }
}
