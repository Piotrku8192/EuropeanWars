using EuropeanWars.Core;
using EuropeanWars.Core.Province;
using EuropeanWars.UI;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.GameMap {
    public enum MapMode {
        Countries,
        Religions,
        Cultures,
        Trade,
        Terrain,
        Recrutation
    }

    public static class MapPainter {
        public static MapMode mapMode;

        public static void PaintMap(MapMode mode) {
            mapMode = mode;
            foreach (var item in GameInfo.provinces) {
                PaintProvince(item.Value, true);
            }
            UpdateBorders();
        }

        public static void UpdateBorders() {
            foreach (var item in GameInfo.provinces) {
                if (item.Value.mapProvince != null) {
                    item.Value.mapProvince.UpdateBorders();
                }
            }
        }

        public static void PaintProvince(ProvinceInfo province, bool b = false) {
            Color color = Color.gray;

            switch (mapMode) {
                case MapMode.Countries:
                    color = province.Country.color;
                    break;
                case MapMode.Religions:
                    color = province.religion.color;
                    break;
                case MapMode.Cultures:
                    color = province.culture.color;
                    break;
                case MapMode.Trade:
                    if (province.isTradeRoute) {
                        color = Color.yellow;
                    }
                    else if (province.isTradeCity) {
                        color = Color.blue;
                    }
                    else {
                        color = Color.gray;
                    }
                    break;
                case MapMode.Terrain:
                    break;
                case MapMode.Recrutation:
                    color = province.claimators.Contains(GameInfo.PlayerCountry) 
                        && province.buildings.Contains(ArmyWindow.Singleton.recrutationWindow.selectedUnit?.recruitBuilding) 
                        && province.Country == GameInfo.PlayerCountry ? Color.green
                        : province.Country == GameInfo.PlayerCountry ? Color.gray : Color.black;
                    break;
                default:
                    break;
            }
            if (province.mapProvince != null && province.isLand) {
                province.mapProvince.material.color = color;
                if (!b) {
                    UpdateBorders();
                }
            }
        }
    }
}
