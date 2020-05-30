using EuropeanWars.Core.Building;
using EuropeanWars.Core.Province;
using EuropeanWars.UI.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class BuildBuildingButton : MonoBehaviour
    {
        public Image image;
        public Text buildingName;

        public BuildingInfo building;

        public void UpdateButton(BuildingInfo building, ProvinceInfo province) {
            this.building = building;
            image.sprite = building.image;
            buildingName.text = building.name;

            if (province != null) {
                GetComponent<Button>().interactable = building.CanBuildInProvince(province);
            }
        }

        public void OnClick() {
            ProvinceWindow.Singleton.BuildBuildingInSlot(building);
        }
    }
}
