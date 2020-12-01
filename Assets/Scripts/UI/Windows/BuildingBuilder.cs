using EuropeanWars.Core;
using EuropeanWars.Core.Province;
using System.Collections.Generic;
using UnityEngine;

namespace EuropeanWars.UI.Windows {
    public class BuildingBuilder : MonoBehaviour {
        public BuildBuildingButton prefab;
        public GameObject builderWindow;
        public GameObject content;
        public List<BuildBuildingButton> buildings = new List<BuildBuildingButton>();

        public void SetWindowActive(bool b) {
            builderWindow.SetActive(b);
            UpdateWindow(ProvinceWindow.Singleton.province);
        }

        public void UpdateWindow(ProvinceInfo province) {
            foreach (var item in buildings) {
                Destroy(item.gameObject);
            }
            buildings.Clear();

            foreach (var item in GameInfo.buildings) {
                if (item.Key > 0) {
                    BuildBuildingButton go = Instantiate(prefab, content.transform);
                    go.UpdateButton(item.Value, province);
                    buildings.Add(go);
                }
            }
        }
    }
}
