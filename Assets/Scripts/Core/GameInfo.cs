﻿using EuropeanWars.Core.AI;
using EuropeanWars.Core.Army;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Culture;
using EuropeanWars.Core.Language;
using EuropeanWars.Core.Persons;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Religion;
using EuropeanWars.GameMap;
using EuropeanWars.UI;
using EuropeanWars.UI.Lobby;
using EuropeanWars.UI.Windows;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EuropeanWars.Core {
    public static class GameInfo {
        public static System.Random random;

        public static Dictionary<string, Sprite> gfx = new Dictionary<string, Sprite>();

        public static Dictionary<int, ProvinceInfo> provinces = new Dictionary<int, ProvinceInfo>();
        public static Dictionary<string, ProvinceInfo> provincesByColor = new Dictionary<string, ProvinceInfo>();

        public static Dictionary<int, BuildingInfo> buildings = new Dictionary<int, BuildingInfo>();
        public static Dictionary<int, ReligionInfo> religions = new Dictionary<int, ReligionInfo>();
        public static Dictionary<int, CultureInfo> cultures = new Dictionary<int, CultureInfo>();
        public static Dictionary<int, CountryInfo> countries = new Dictionary<int, CountryInfo>();
        public static Dictionary<int, UnitInfo> units = new Dictionary<int, UnitInfo>();

        public static Dictionary<int, Person> persons = new Dictionary<int, Person>();

        public static Dictionary<int, ArmyInfo> armies = new Dictionary<int, ArmyInfo>();
        public static Dictionary<int, MercenariesInfo> mercenaries = new Dictionary<int, MercenariesInfo>();

        public static Dictionary<CountryInfo, CountryAI> countryAIs = new Dictionary<CountryInfo, CountryAI>();

        public static ProvinceInfo SelectedProvince { get; private set; }
        public static CountryInfo PlayerCountry { get; private set; }
        public static bool gameStarted;

        public static void Initialize() {
            //random = new System.Random(69); //hahaha very funny seed lol ~PZFN 20.07.2020 20:40 ;) (im stupid man i guess)
            random = new System.Random(1423415);// System.DateTime.Now.Millisecond);

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
            foreach (var item in mercenaries) {
                item.Value.Initialize();
            }

            ChangeLanguage(0);
        }

        public static void ChangeLanguage(int id) {
            LanguageDictionary.language = LanguageDictionary.languages[id];

            foreach (var item in buildings) {
                item.Value.UpdateLanguage();
            }
            foreach (var item in religions) {
                item.Value.UpdateLanguage();
            }
            foreach (var item in cultures) {
                item.Value.UpdateLanguage();
            }
            foreach (var item in provinces) {
                item.Value.UpdateLanguage();
            }
            foreach (var item in countries) {
                item.Value.UpdateLanguage();
            }
            foreach (var item in units) {
                item.Value.UpdateLanguage();
            }
            foreach (var item in mercenaries) {
                item.Value.UpdateLanguage();
            }

            TranslatedDescriptionsLibrary.Singleton.UpdateLanguage();
            DiplomacyWindow.Singleton.UpdateLanguage();
            TradeWindow.Singleton.UpdateLanguage();
        }

        public static void SetPlayerCountry(CountryInfo country) {
            PlayerCountry = country;
            foreach (var item in PlayerCountry.relations) {
                item.Value.withPlayerCountry = true;
            }
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
                    ProvinceWindow.Singleton.UpdateWindow(SelectedProvince, true);
                }

                if (MapPainter.mapMode == MapMode.Relations || MapPainter.mapMode == MapMode.Diplomatic) {
                    MapPainter.PaintMap(MapPainter.mapMode);
                }
            }
        }

        public static void UnselectProvince() {
            SelectedProvince?.mapProvince.OnUnselectProvince();
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

        public static bool IsPointerOverScrollView() {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current) {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Where(t => t.gameObject.GetComponent<ScrollRect>() != null).Any();
        }
    }
}
