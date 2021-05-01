using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Province;
using System;
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
        public Text artillerySize;
        public Image artillerySizeBackground;
        public Image sizeBackground;
        public GameObject blackStatusImage;
        public GameObject occupationProgress;
        public Image occupationProgressBar;
        public Text occupationProgressText;

        public float scale;

        public bool isMovementCoroutineExecuting;

        public void Initialize(ArmyInfo army) {
            this.army = army;
            crest.sprite = army.Country.Crest;
            transform.position = new Vector3(army.Province.x, army.Province.y, 0);
            if (army.isNavy) {
                artillerySizeBackground.color = Color.cyan;
            }
        }

        public void Update() {
            if (GameInfo.gameStarted) {
                if (army.isNavy) {
                    size.text = $"{Math.Round(army.Size * 0.001f, 1)}k";
                    artillerySize.text = army.units.Where(t => t.Key.type == UnitType.Navy).Sum(t => t.Value).ToString();
                    UpdateScale();
                    UpdateColor();
                }
                else {
                    size.text = $"{Math.Round(army.Size * 0.001f, 1)}k";
                    artillerySize.text = army.Artilleries.ToString();
                    UpdateScale();
                    UpdateColor();
                    blackStatusImage.SetActive(army.BlackStatus);

                    if (army.Province.OccupationCounter?.Army == army) {
                        occupationProgress.SetActive(true);
                        occupationProgressBar.fillAmount = army.Province.OccupationCounter.Progress / 100;
                        occupationProgressText.text = Mathf.RoundToInt(army.Province.OccupationCounter.Progress) + "%";
                    }
                    else {
                        occupationProgress.SetActive(false);
                    }
                }
            }
        }

        public IEnumerator MoveObjectToProvince(ProvinceInfo oldProvince, ProvinceInfo newProvince) {
            Vector2 pos1 = new Vector2(oldProvince.x, oldProvince.y);
            Vector2 pos2 = new Vector2(newProvince.x, newProvince.y);

            for (int i = 0; i <= 50; i++) {
                Vector2 p = Vector2.Lerp(pos1, pos2, (float)i/50);

                transform.position = p;
                if (army.Country == GameInfo.PlayerCountry) {
                    lineRenderer.SetPosition(0, p);
                }

                yield return new WaitForFixedUpdate();
            }

            if (army.Country == GameInfo.PlayerCountry) {
                DrawRoute(army.route.ToArray());
            }
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

        public void UpdateColor() {
            Color color = Color.gray;

            if (army.Country == GameInfo.PlayerCountry) {
                color = Color.green;
            }
            else if (GameInfo.PlayerCountry.vassals.Contains(army.Country)) {
                color = Color.magenta;
            }
            else if (GameInfo.PlayerCountry.IsInWarAgainstCountry(army.Country)) {
                color = Color.red;
            }
            else if (GameInfo.PlayerCountry.IsInWarWithCountry(army.Country)) {
                color = Color.blue;
            }
            else if (GameInfo.PlayerCountry.relations[army.Country].relations[(int)DiplomaticRelation.Alliance]) {
                color = Color.cyan;
            }

            sizeBackground.color = color;
        }

        public void DrawRoute(ProvinceInfo[] route) {
            try {
                lineRenderer.positionCount = route.Length;
                for (int i = 0; i < route.Length; i++) {
                    ProvinceInfo item = route[i];
                    lineRenderer.SetPosition(i, new Vector2(item.x, item.y));
                }
            }
            catch {

            }
        }
    }
}
