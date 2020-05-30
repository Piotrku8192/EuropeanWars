using EuropeanWars.Core;
using EuropeanWars.Core.Province;
using UnityEngine;

namespace EuropeanWars.GameMap {
    public enum MapMode {
        Countries,
        Religions,
        Cultures,
        Trade,
        Terrain
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
                    color = province.isTradeRoute ? Color.yellow : Color.gray;
                    color = province.isTradeCity ? Color.blue : Color.gray;
                    break;
                case MapMode.Terrain:
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
