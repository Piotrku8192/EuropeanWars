using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EuropeanWars.UI {
    public class DescriptionMouseText : MonoBehaviour {
        public static DescriptionMouseText Singleton { get; private set; }
        private RectTransform rectTransform;

        public Text text;

        public DescriptionText description;

        public void Awake() {
            Singleton = this;
            rectTransform = GetComponent<RectTransform>();
        }

        public void Update() {
            if (!IsPointerOverUIElement() || description == null) {
                transform.localScale = new Vector3();
                text.gameObject.SetActive(false);
                text.text = "";
            }
            else if (description != null) {
                transform.position = Input.mousePosition;

                transform.localScale = new Vector3(1, 1, 1);
                text.gameObject.SetActive(true);
                text.text = description.text;
                //rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,
                //    lineHeight * (Mathf.FloorToInt(text.text.Length / lineLength) + 1) + padding);

                if (transform.position.x + rectTransform.sizeDelta.x > Screen.width
                    && transform.position.y - rectTransform.sizeDelta.y < 0) {
                    rectTransform.pivot = new Vector2(1, 0);
                }
                else if (transform.position.x + rectTransform.sizeDelta.x > Screen.width
                    && transform.position.y - rectTransform.sizeDelta.y > 0) {
                    rectTransform.pivot = new Vector2(1, 1);
                }
                else if (transform.position.x + rectTransform.sizeDelta.x < Screen.width
                    && transform.position.y - rectTransform.sizeDelta.y < 0) {
                    rectTransform.pivot = new Vector2(0, 0);
                }
                else {
                    rectTransform.pivot = new Vector2(-(15 / rectTransform.sizeDelta.x), 1);
                }
            }
        }

        public static bool IsPointerOverUIElement() {
            return IsPointerOverUIElement(GetEventSystemRaycastResults());
        }
        public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults) {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++) {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("OnMouseText")) {
                    return true;
                }
            }
            return false;
        }

        private static List<RaycastResult> GetEventSystemRaycastResults() {
            PointerEventData eventData = new PointerEventData(EventSystem.current) {
                position = Input.mousePosition
            };
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
    }
}
