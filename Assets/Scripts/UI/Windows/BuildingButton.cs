using EuropeanWars.Core;
using EuropeanWars.Core.Building;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class BuildingButton : MonoBehaviour {
        public int slotId;
        public Image image;
        public Image buildingProgress;

        public BuildingInfo building;

        public void UpdateButton(BuildingInfo building) {
            this.building = building;
            image.sprite = building.image;
            buildingProgress.gameObject.SetActive(false);
            //buildingProgress.fillAmount = progress / (float)building.buildingTime;
        }

        public void OnClick() {
            if (ProvinceWindow.Singleton.selectedBuildingSlot == slotId && ProvinceWindow.Singleton.builder.builderWindow.activeInHierarchy) {
                ProvinceWindow.Singleton.builder.SetWindowActive(false);
                return;
            }

            if (ProvinceWindow.Singleton.province.Country == GameInfo.PlayerCountry) {
                if (building.id == 0) { 
                    ProvinceWindow.Singleton.SelectBuildingSlot(slotId);
                }
                else {
                    DeleteBuildingWindow w = Instantiate(ProvinceWindow.Singleton.deleteBuildingWindowPrefab, UIManager.Singleton.ui.transform);
                    w.provinceId = ProvinceWindow.Singleton.province.id;
                    w.slotId = slotId;
                }
            }
        }
    }
}
