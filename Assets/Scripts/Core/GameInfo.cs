using EuropeanWars.Core.Pathfinding;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Culture;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Religion;
using EuropeanWars.UI.Lobby;
using EuropeanWars.UI.Windows;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using EuropeanWars.Core.Army;

namespace EuropeanWars.Core {
    public static class GameInfo {
        public static Dictionary<string, Sprite> gfx = new Dictionary<string, Sprite>();

        public static Dictionary<int, ProvinceInfo> provinces = new Dictionary<int, ProvinceInfo>();
        public static Dictionary<string, ProvinceInfo> provincesByColor = new Dictionary<string, ProvinceInfo>();

        public static Dictionary<int, BuildingInfo> buildings = new Dictionary<int, BuildingInfo>();
        public static Dictionary<int, ReligionInfo> religions = new Dictionary<int, ReligionInfo>();
        public static Dictionary<int, CultureInfo> cultures = new Dictionary<int, CultureInfo>();
        public static Dictionary<int, CountryInfo> countries = new Dictionary<int, CountryInfo>();
        public static Dictionary<int, UnitInfo> units = new Dictionary<int, UnitInfo>();

        public static Dictionary<int, ArmyInfo> armies = new Dictionary<int, ArmyInfo>();

        public static ProvinceInfo SelectedProvince { get; private set; }
        public static CountryInfo PlayerCountry { get; private set; }
        public static bool gameStarted;

        public static void Initialize() {
            foreach (var item in buildings) {
                item.Value.Initialize();
            }
            foreach (var item in religions) {
                item.Value.Initialize();
            }
            foreach (var item in cultures) {
                item.Value.Initialize();
            }
            foreach (var item in provinces) {
                item.Value.Initialize();
            }
            foreach (var item in countries) {
                item.Value.Initialize();
            }
            foreach (var item in units) {
                item.Value.Initialize();
            }
        }

        public static void SetPlayerCountry(CountryInfo country) {
            PlayerCountry = country;
        }

        public static void SelectProvince(ProvinceInfo province) {
            if (SelectedProvince != province) {
                SelectedProvince?.mapProvince.OnUnselectProvince();
                SelectedProvince = province;
                SelectedProvince.mapProvince.OnSelectProvince();

                if (LobbyManager.Singleton.gameObject.activeInHierarchy && province.Country.id != 0 && !gameStarted) {
                    LobbyManager.Singleton.SelectCountry(province.Country);
                }

                if (gameStarted) {
                    ProvinceWindow.Singleton.UpdateWindow(SelectedProvince);
                }
            }
        }

        public static void UnselectProvince() {
            SelectedProvince.mapProvince.OnSelectProvince();
            SelectedProvince = null;
        }

        public static bool IsPointerOverUIObject() {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current) {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Count > 0;
        }
    }
}
