using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.Core.Army {
    public class ArmyRectangleSelector : MonoBehaviour {
        public Vector2 startPoint;
        private Image image;
        private bool isActive;

        private RectTransform rect;

        public void Start() {
            image = GetComponent<Image>();
            rect = GetComponent<RectTransform>();
        }

        public void Update() {
            if (!GameInfo.gameStarted) {
                return;
            }

            //if (!GameInfo.IsPointerOverUIObject()) {
            Vector2 point = Controller.Singleton.playerCam.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetKeyDown(KeyCode.Mouse1) && !isActive) {
                isActive = true;
                startPoint = point;
            }

            if (Input.GetKey(KeyCode.Mouse1) && isActive && point != startPoint) {
                Vector2 start = Vector2.Min(point, startPoint);
                Vector2 end = Vector2.Max(point, startPoint);
                float y = start.y;
                start.y = end.y;
                end.y = y;
                rect.sizeDelta = new Vector2(Mathf.Abs(end.x - start.x), Mathf.Abs(end.y - start.y));
                rect.anchoredPosition = start;
                image.enabled = true;
            }
            //}

            if (Input.GetKeyUp(KeyCode.Mouse1) && isActive) {
                isActive = false;
                image.enabled = false;

                if (!Input.GetKey(KeyCode.LeftShift)) {
                    ArmyInfo.UnselectAll();
                }
                foreach (var item in GameInfo.PlayerCountry.armies) {
                    Rect r = new Rect(rect.anchoredPosition.x, rect.anchoredPosition.y - rect.sizeDelta.y,
                        rect.sizeDelta.x, rect.sizeDelta.y);
                    if (r.Contains(item.ArmyObject.transform.position)) {
                        item.SelectArmy(false);
                    }
                }

                rect.sizeDelta = new Vector2();
            }
        }
    }
}
