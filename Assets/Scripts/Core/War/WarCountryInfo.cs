using EuropeanWars.Core.Country;
using EuropeanWars.Core.Province;
using EuropeanWars.UI.Windows;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.Core.War {
    public class WarCountryInfo {
        public readonly CountryInfo country;
        public readonly WarParty party;

        public readonly List<ProvinceInfo> enemyOccupatedProvinces = new List<ProvinceInfo>();
        public readonly List<ProvinceInfo> localOccupatedProvinces = new List<ProvinceInfo>();
        //TODO: Add battles

        public int killedEnemies;
        public int killedLocal;

        public bool IsMajor => party.major == this;

        public int CountryScoreCost => country.nationalProvinces.Sum(t => t.taxation);
        public int WarScore { get; private set; }
        public int PercentWarScore => Mathf.RoundToInt((float)WarScore / (WarScore < 0 ? CountryScoreCost : party.Enemies.PartyScoreCost) * 100);
        public Color PercentWarScoreColor => PercentWarScore == 0 ? Color.yellow : (PercentWarScore > 0 ? Color.green : Color.red);

        public WarCountryInfo(CountryInfo country, WarParty party) {
            this.country = country;
            this.party = party;
        }

        /// <summary>
        /// Automatically invokes AddLocalOccupatedProvince in occupated Country.
        /// </summary>
        /// <param name="province"></param>
        public void AddEnemyOccupatedProvince(ProvinceInfo province) {
            if (!enemyOccupatedProvinces.Contains(province)
                && province.Country == country && party.Enemies.ContainsCountry(province.NationalCountry)) {
                enemyOccupatedProvinces.Add(province);
                WarScore += province.taxation;
                party.Enemies.countries[province.NationalCountry].AddLocalOccupatedProvince(province);

                UpdateDynamicPeaceDealOnAddEnemyProvince(province);
            }
        }

        /// <summary>
        /// Automatically invokes RemoveLocalOccupatedProvince in occupated Country.
        /// </summary>
        /// <param name="province"></param>
        public void RemoveEnemyOccupatedProvince(ProvinceInfo province) {
            if (enemyOccupatedProvinces.Contains(province) && province.Country == province.NationalCountry) {
                enemyOccupatedProvinces.Remove(province);
                WarScore -= province.taxation;
                if (party.Enemies.countries.ContainsKey(province.NationalCountry)) {
                    party.Enemies.countries[province.NationalCountry]?.RemoveLocalOccupatedProvince(province);
                }

                UpdateDynamicPeaceDealOnRemoveEnemyProvince(province);
            }
        }

        /// <summary>
        /// Doesn't invoke AddEnemyOccupatedProvince in occupant.
        /// </summary>
        /// <param name="province"></param>
        public void AddLocalOccupatedProvince(ProvinceInfo province) {
            if (!localOccupatedProvinces.Contains(province) 
                && province.NationalCountry == country && party.Enemies.ContainsCountry(province.Country)) {
                localOccupatedProvinces.Add(province);
                WarScore -= province.taxation;
            }
        }

        /// <summary>
        /// Doesn't invoke RemoveEnemyOccupatedProvince in occupant.
        /// </summary>
        /// <param name="province"></param>
        public void RemoveLocalOccupatedProvince(ProvinceInfo province) {
            if (localOccupatedProvinces.Contains(province) && province.Country == province.NationalCountry && province.Country == country) {
                localOccupatedProvinces.Remove(province);
                WarScore += province.taxation;
            }
        }

        private void UpdateDynamicPeaceDealOnAddEnemyProvince(ProvinceInfo province) {
            DynamicPeaceDeal deal = PeaceDealWindow.Singleton.peaceDeal;
            if (deal?.war == party.war) {
                if (deal.sender == this
                    && (deal.receiver.country == province.NationalCountry || deal.receiver.IsMajor)) {
                    ProvincePeaceDealElement e = new ProvincePeaceDealElement(province, deal);
                    deal.senderElements.Add(deal.nextElementId, e);
                    deal.nextElementId++;
                    PeaceDealWindow.Singleton.AddSenderElement(e);
                }
                else if (deal.receiver == this
                    && (deal.sender.country == province.NationalCountry || deal.sender.IsMajor)) {
                    ProvincePeaceDealElement e = new ProvincePeaceDealElement(province, deal);
                    deal.receiverElements.Add(deal.nextElementId, e);
                    deal.nextElementId++;
                    PeaceDealWindow.Singleton.AddReceiverElement(e);
                }
            }
        }

        private void UpdateDynamicPeaceDealOnRemoveEnemyProvince(ProvinceInfo province) {
            DynamicPeaceDeal deal = PeaceDealWindow.Singleton.peaceDeal;
            if (deal?.war == party.war) {
                if (deal.sender == this
                    && (deal.receiver.country == province.NationalCountry || deal.receiver.IsMajor)) {
                    int id = -1;
                    foreach (var item in deal.senderElements) {
                        if (item.Value is ProvincePeaceDealElement e) {
                            if (e.province == province) {
                                id = item.Key;
                                break;
                            }
                        }
                    }
                    if (id != -1) {
                        PeaceDealWindow.Singleton.RemoveSenderElement(deal.senderElements[id]);
                        deal.UnselectSenderElement(deal.senderElements[id]);
                        deal.senderElements.Remove(id);
                    }
                }
                else if (deal.receiver == this
                    && (deal.sender.country == province.NationalCountry || deal.sender.IsMajor)) {
                    int id = -1;
                    foreach (var item in deal.receiverElements) {
                        if (item.Value is ProvincePeaceDealElement e) {
                            if (e.province == province) {
                                id = item.Key;
                                break;
                            }
                        }
                    }
                    if (id != -1) {
                        PeaceDealWindow.Singleton.RemoveReceiverElement(deal.receiverElements[id]);
                        deal.UnselectReceiverElement(deal.receiverElements[id]);
                        deal.receiverElements.Remove(id);
                    }
                }
            }
        }
    }
}
