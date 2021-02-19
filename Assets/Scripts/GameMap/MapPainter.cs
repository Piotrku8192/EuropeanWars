using EuropeanWars.Core;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.War;
using EuropeanWars.UI;
using EuropeanWars.UI.Windows;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.GameMap {
    public enum MapMode {
        Countries,
        Religions,
        Cultures,
        Trade,
        Terrain,
        Recrutation,
        Peace,
        Relations,
        Diplomatic
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
                case MapMode.Peace:
                    PeaceDeal peaceDeal = PeaceDealWindow.Singleton.peaceDeal;

                    if (peaceDeal != null) {
                        if (peaceDeal.war.ContainsCountry(province.NationalCountry)) {
                            color = province.NationalCountry.color;
                        }

                        foreach (var item in peaceDeal.senderElements) {
                            if (item.Value is ProvincePeaceDealElement p) {
                                if (p.province == province) {
                                    if (peaceDeal.selectedSenderElements.Contains(item.Key)) {
                                        color = Color.green;
                                    }
                                    else {
                                        if (item.Value.CanBeSelected(peaceDeal)) {
                                            color = Color.yellow;
                                        }
                                        else {
                                            color = new Color32(232, 112, 0, 255);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        foreach (var item in peaceDeal.receiverElements) {
                            if (item.Value is ProvincePeaceDealElement p) {
                                if (p.province == province) {
                                    if (peaceDeal.selectedReceiverElements.Contains(item.Key)) {
                                        color = Color.red;
                                    }
                                    else {
                                        if (item.Value.CanBeSelected(peaceDeal)) {
                                            color = Color.magenta;
                                        }
                                        else {
                                            color = new Color32(157, 0, 255, 255);
                                        }
                                    }
                                    break;
                                }
                            }
                        }

                    }
                    break;
                case MapMode.Relations:
                    if (GameInfo.SelectedProvince != null) {
                        if (GameInfo.SelectedProvince.Country != province.Country) {
                            color = Color.Lerp(Color.red, Color.green, (GameInfo.SelectedProvince.Country.relations[province.Country].Points + 100) / 200.0f);
                            break;
                        }
                    }
                    else {
                        if (GameInfo.PlayerCountry != province.Country) {
                            color = Color.Lerp(Color.red, Color.green, (GameInfo.PlayerCountry.relations[province.Country].Points + 100) / 200.0f);
                            break;
                        }
                    }
                    color = Color.gray;
                    break;
                case MapMode.Diplomatic:
                    CountryInfo country;
                    if (GameInfo.SelectedProvince != null) {
                        country = GameInfo.SelectedProvince.Country;
                    }
                    else {
                        country = GameInfo.PlayerCountry;
                    }

                    if (country == province.Country) {
                        color = Color.green;
                    }
                    else {
                        CountryRelation relation = country.relations[province.Country];
                        if (country.IsInWarAgainstCountry(province.Country)) {
                            color = Color.red;
                        }
                        else if (relation.relations[(int)DiplomaticRelation.Alliance] == true) {
                            color = Color.blue;
                        }
                        else if (country.vassals.Contains(province.Country)) {
                            color = Color.cyan;
                        }
                    }
                    break;
                default:
                    break;
            }
            if (GameInfo.gameStarted) {
                if (mapMode == MapMode.Countries) {
                    province.RefreshFogOfWar();
                }
                else {
                    province.mapProvince?.material?.SetFloat("_FogOfWar", 0);
                }
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
