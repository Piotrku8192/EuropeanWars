using EuropeanWars.Core;
using EuropeanWars.Core.Province;
using EuropeanWars.GameMap.Data;
using EuropeanWars.Province;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.GameMap {
    public class MapGenerator : MonoBehaviour {
        public static MapGenerator Singleton { get; private set; }

        public Material provinceBorderMaterial;
        public Material countryBorderMaterial;

        public Material provinceMaterial;
        public Material countriesMapMaterial;
        public Material waterMaterial;

        public MapData mapData;
        public Dictionary<string, List<Border>> borders = new Dictionary<string, List<Border>>();

        public float countriesMapDistance;
        public float farCountriesMapDistance;

        public void Awake() {
            Singleton = this;
        }

        public void GenerateMap() {
            foreach (var item in mapData.borders) {
                GameObject go = new GameObject(item.firstProvince + item.secondProvince, typeof(Border));
                go.transform.SetParent(transform);
                Border b = go.GetComponent<Border>();
                if (GameInfo.provincesByColor.ContainsKey(item.firstProvince) && GameInfo.provincesByColor.ContainsKey(item.firstProvince)) {
                    b.GenerateBorder(item);
                    if (!borders.ContainsKey(b.province1.color)) {
                        borders.Add(b.province1.color, new List<Border>());
                    }
                    if (!borders.ContainsKey(b.province2.color)) {
                        borders.Add(b.province2.color, new List<Border>());
                    }
                    borders[b.province1.color].Add(b);
                    borders[b.province2.color].Add(b);
                }

            }

            foreach (var item in mapData.provinces) {
                if (borders.ContainsKey(item.color)) {
                    GameObject go = new GameObject(item.color, typeof(MapProvince));
                    go.transform.SetParent(transform);
                    MapProvince p = go.GetComponent<MapProvince>();
                    p.borders.AddRange(borders[item.color]);
                    ProvinceInfo province = GameInfo.provinces.Where(t => t.Value.color == item.color).FirstOrDefault().Value;
                    p.GenerateProvince(item.mesh, province.isLand ? new Material(provinceMaterial) : waterMaterial, new Material(countriesMapMaterial), province);
                    province.mapProvince = p;

                    //TODO: Delete this temporary code plz!
                    foreach (var b in p.borders) {
                        province.neighbours.Add(b.province1 == province ? b.province2 : b.province1);
                    }
                }
            }
        }
    }
}
