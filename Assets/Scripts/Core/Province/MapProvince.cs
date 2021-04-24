using EuropeanWars.Core;
using EuropeanWars.Core.Army;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.War;
using EuropeanWars.GameMap;
using EuropeanWars.GameMap.Data;
using EuropeanWars.Network;
using EuropeanWars.UI;
using EuropeanWars.UI.Windows;
using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EuropeanWars.Province {
    public class MapProvince : MonoBehaviour {
        public List<Border> borders = new List<Border>();
        public MeshFilter filter;
        public MeshRenderer meshRenderer;
        public MeshCollider meshCollider;
        public Material material;
        public Material countriesMapMaterial;

        private ProvinceInfo provinceInfo;
        private Coroutine selectionCoroutine;

        public void OnMouseDown() {
            if (!GameInfo.IsPointerOverUIObject()) {
                if (MapPainter.mapMode == MapMode.Recrutation) {
                    RecruitRegularArmy();
                    return;
                }
                else if (MapPainter.mapMode == MapMode.Peace) {
                    foreach (var item in PeaceDealWindow.Singleton.senderElements) {
                        if (item.Key is ProvincePeaceDealElement p) {
                            if (p.province == provinceInfo) {
                                item.Value.OnClick();
                            }
                        }
                    }
                    foreach (var item in PeaceDealWindow.Singleton.receiverElements) {
                        if (item.Key is ProvincePeaceDealElement p) {
                            if (p.province == provinceInfo) {
                                item.Value.OnClick();
                            }
                        }
                    }
                    return;
                }
                GameInfo.SelectProvince(provinceInfo);
                foreach (var item in ArmyInfo.selectedArmies) {
                    item.GenerateRouteRequest(provinceInfo);
                }
            }
        }

        public void Update() {
            if (countriesMapMaterial.color != material.color) {
                countriesMapMaterial.color = material.color;
            }
        }

        public void ChangeToCountriesMap() {
            if (meshRenderer.material != countriesMapMaterial && provinceInfo.isLand) {
                meshRenderer.material = countriesMapMaterial;
            }
        }

        public void ChangeToProvincesMap() {
            if (meshRenderer.material != material) {
                meshRenderer.material = material;
            }
        }

        public void GenerateProvince(MeshData meshData, Material material, Material countriesMaterial, ProvinceInfo provinceInfo) {
            this.provinceInfo = provinceInfo;
            filter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            this.material = material;
            countriesMapMaterial = countriesMaterial;
            meshRenderer.material = provinceInfo.isLand ? countriesMaterial : material;

            Mesh m = new Mesh();
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();

            foreach (var v in meshData.vertices) {
                verts.Add(new Vector3(v.X, -v.Y));
                uv.Add(new Vector2(v.X, -v.Y));
            }

            Vector3 min = verts[0], max = verts[0];
            foreach (var v in verts) {
                if (v.x < min.x) {
                    min.x = v.x;
                }
                if (v.y > min.y) {
                    min.y = v.y;
                }
                if (v.x > max.x) {
                    max.x = v.x;
                }
                if (v.y < max.y) {
                    max.y = v.y;
                }
            }
            //TODO: Maybe do something with that
            provinceInfo.x = Mathf.Lerp(min.x, max.x, 0.5f);
            provinceInfo.y = Mathf.Lerp(min.y, max.y, 0.5f);

            m.vertices = verts.ToArray();
            m.uv = uv.ToArray();
            m.triangles = meshData.indices.ToArray();

            filter.sharedMesh = m;
            meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = m;

            gameObject.SetActive(provinceInfo.isActive);
            foreach (var item in borders) {
                item.gameObject.SetActive(provinceInfo.isActive);
            }
        }

        public void UpdateBorders() {
            foreach (var item in borders) {
                item.UpdateBorder();
            }
        }

        public void OnSelectProvince() {
            selectionCoroutine = StartCoroutine(UpdateSelection());
            foreach (var item in borders) {
                item.SetStyle(BorderStyle.SelectedProvince);
            }
        }

        public void OnUnselectProvince() {
            if (selectionCoroutine != null) {
                StopCoroutine(selectionCoroutine);
            }
            foreach (var item in borders) {
                item.SetColor(Color.black);
            }
            UpdateBorders();
        }

        public IEnumerator UpdateSelection() {
            int i = 0;
            int max = 50;
            Color from = Color.yellow * 0.25f;
            from.a = 1.0f;
            Color to = Color.yellow;
            while (true) {
                if (i > max) {
                    i = 0;
                    Color f = from;
                    from = to;
                    to = f;
                }
                foreach (var item in borders) {
                    item.SetColor(Color.Lerp(from, to, i / (float)max));
                }

                i++;
                yield return new WaitForFixedUpdate();
            }
        }

        public void RecruitRegularArmy() {
            if (ArmyWindow.Singleton.recrutationWindow.recruitSizeSlider.value > 0) {
                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)2048);
                msg.Write(ArmyWindow.Singleton.recrutationWindow.selectedUnit.id);
                msg.Write(GameInfo.PlayerCountry.id);
                msg.Write(provinceInfo.id);
                msg.Write(Mathf.RoundToInt(ArmyWindow.Singleton.recrutationWindow.recruitSizeSlider.value));
                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
            }
        }
    }
}
