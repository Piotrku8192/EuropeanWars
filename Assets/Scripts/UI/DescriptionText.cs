using UnityEngine;
using UnityEngine.EventSystems;

namespace EuropeanWars.UI {
    public class DescriptionText : MonoBehaviour, IPointerEnterHandler {
        [TextArea]
        public string text;

        public void OnPointerEnter(PointerEventData eventData) {
            DescriptionMouseText.Singleton.description = this;
        }
    }
}
