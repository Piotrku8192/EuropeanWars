using EuropeanWars.Core.Province;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.Core.Army {
    public class ArmyObject : MonoBehaviour {
        public ArmyInfo army;
        public LineRenderer lineRenderer;

        public GameObject gui;
        public Image selectionOutline;
        public Image crest;
        public Text size;
        public Image sizeBackground;

        public float scale;

        public void Initialize(ArmyInfo army) {
            this.army = army;
            crest.sprite = army.Country.crest;
            transform.position = new Vector3(army.Province.x, army.Province.y, 0);
        }

        public void Update() {
            size.text = $"{army.Size * 0.001f}k";
            UpdateScale();
        }

        public void OnClick() {
            if (army.IsSelected) {
                army.UnselectArmy();
            }
            else {
                army.SelectArmy(!Input.GetKey(KeyCode.LeftShift));
            }
        }

        public void UpdateScale() {
            float curDistance = Controller.Singleton.playerCam.orthographicSize;

            if (curDistance > Controller.Singleton.armiesDistance || army.Province.fogOfWar) {
                gui.SetActive(false);
                lineRenderer.enabled = false;
            }
            else {
                gui.SetActive(true);
                lineRenderer.enabled = true;
            }
            transform.localScale = new Vector3(1, 1, 1) * scale * curDistance / Controller.Singleton.minScope;
        }

        public void DrawRoute(ProvinceInfo[] route) {
            lineRenderer.positionCount = route.Length;
            lineRenderer.SetPosition(0, transform.position);

            for (int i = 0; i < route.Length; i++) {
                ProvinceInfo item = route[i];
                lineRenderer.SetPosition(i, new Vector2(item.x, item.y));
            }
        }

        public void CountMovement() {
            try {
                StartCoroutine(CountMovementCoroutine());
            }
            catch {

            }
        }

        private IEnumerator CountMovementCoroutine() {
            if (army.route.Count > 1) {
                Vector3 x = transform.position;
                Vector3 y = new Vector3(army.route.ToArray()[1].x, army.route.ToArray()[1].y);

                if (x == y) {
                    army.route.Dequeue();
                    if (army.Country == GameInfo.PlayerCountry) {
                        DrawRoute(army.route.ToArray());
                    }
                }

                for (int i = 0; i < 100; i++) {
                    yield return new WaitForFixedUpdate();
                    transform.position = Vector3.MoveTowards(transform.position, y, army.AverageSpeed * .007f);
                    if (army.Country == GameInfo.PlayerCountry) {
                        try {
                            lineRenderer.SetPosition(0, transform.position);
                        }
                        catch {

                        }
                    }

                    if (Vector3.Distance(transform.position, y) < Vector3.Distance(new Vector2(army.Province.x, army.Province.y), y) * 0.5f) {
                        if (army.route.Count > 1 && army.Province != army.route.ToArray()[1]) {
                            army.OnArmyMove(army.route.ToArray()[1]);
                        }
                    }
                }
            }
        }
    }
}
