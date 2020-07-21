using EuropeanWars.Core;
using EuropeanWars.Core.Province;
using EuropeanWars.GameMap.Data;
using System.Collections.Generic;
using UnityEngine;

namespace EuropeanWars.GameMap {
    public enum BorderStyle {
        BetweenProvinces,
        BetweenCountries,
        SelectedProvince
    }

    public class Border : MonoBehaviour {
        public List<LineRenderer> lines = new List<LineRenderer>();

        public ProvinceInfo province1, province2;
        public BorderStyle style;

        private bool isGenerated = false;

        public void Update() {
            if (isGenerated) {
                if (style == BorderStyle.BetweenProvinces) {
                    if (Controller.Singleton.playerCam.orthographicSize > MapGenerator.Singleton.countriesMapDistance) {
                        foreach (var item in lines) {
                            item.enabled = false;
                        }
                    }
                    else {
                        foreach (var item in lines) {
                            item.enabled = true;
                        }
                    }
                }
            }
        }

        public void GenerateBorder(BorderData borderData) {
            province1 = GameInfo.provincesByColor[borderData.firstProvince];
            province2 = GameInfo.provincesByColor[borderData.secondProvince];
            //province1.neighbours.Add(province2);
            //province2.neighbours.Add(province1);

            foreach (var verts in borderData.vertices) {
                GameObject go = new GameObject(borderData.firstProvince + borderData.secondProvince);
                go.transform.SetParent(transform);
                LineRenderer line = go.AddComponent<LineRenderer>();
                lines.Add(line);
                List<Vector3> positions = new List<Vector3>();

                foreach (var vert in verts) {
                    positions.Add(new Vector3(vert.X, -vert.Y, 0));
                }

                line.sortingOrder = 5;
                line.positionCount = verts.Count;
                line.SetPositions(positions.ToArray());
                line.material = MapGenerator.Singleton.provinceBorderMaterial;
                line.startWidth = 1f;
                line.endWidth = 1f;
                line.numCornerVertices = 0;
                line.numCapVertices = 3;
                line.textureMode = LineTextureMode.Tile;
                SetColor(Color.black);
            }
        }

        public void UpdateBorder() {
            if (province1.mapProvince.material.color == province2.mapProvince.material.color) {
                SetStyle(BorderStyle.BetweenProvinces);
            }
            else {
                SetStyle(BorderStyle.BetweenCountries);
            }
            isGenerated = true;
        }

        public void SetStyle(BorderStyle style) {
            this.style = style;
            switch (style) {
                case BorderStyle.BetweenProvinces:
                    SetMaterial(MapGenerator.Singleton.provinceBorderMaterial);
                    SetWidth(0.7f);
                    break;
                case BorderStyle.BetweenCountries:
                    if (isGenerated) {
                        foreach (var item in lines) {
                            item.enabled = true;
                        }
                    }
                    SetMaterial(MapGenerator.Singleton.countryBorderMaterial);
                    SetWidth(2);//1.4f);
                    break;
                case BorderStyle.SelectedProvince:
                    foreach (var item in lines) {
                        item.enabled = true;
                    }
                    SetMaterial(MapGenerator.Singleton.countryBorderMaterial);
                    SetWidth(1.0f);
                    break;
                default:
                    break;
            }
        }

        public void SetColor(Color color) {
            foreach (var item in lines) {
                item.startColor = color;
                item.endColor = color;
            }
        }

        public void SetWidth(float width) {
            foreach (var item in lines) {
                item.widthMultiplier = width;
            }
        }
        public void SetMaterial(Material material) {
            foreach (var item in lines) {
                item.material = material;
            }
        }
    }
}
