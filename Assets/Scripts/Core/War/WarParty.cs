using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Province;
using EuropeanWars.UI.Windows;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.Core.War {
    public class WarParty {
        public readonly WarInfo war;
        public WarParty Enemies { get; private set; }
        public readonly WarCountryInfo major;
        public readonly Dictionary<CountryInfo, WarCountryInfo> countries;

        public int PartyScoreCost => countries.Sum(t => t.Value.CountryScoreCost);
        public int WarScore => countries.Sum(t => t.Value.WarScore);
        public int PercentWarScore => Mathf.RoundToInt((float)WarScore / (WarScore < 0 ? PartyScoreCost : Enemies.PartyScoreCost) * 100);
        public Color PercentWarScoreColor => PercentWarScore == 0 ? Color.yellow : (PercentWarScore > 0 ? Color.green : Color.red);

        /// <summary>
        /// After making parties you must set Enemies party by invoking SetEnemies method.
        /// </summary>
        /// <param name="war"></param>
        /// <param name="major"></param>
        public WarParty(WarInfo war, CountryInfo major) {
            this.war = war;
            this.major = new WarCountryInfo(major, this);
            countries = new Dictionary<CountryInfo, WarCountryInfo>();
        }

        /// <summary>
        /// Must be setted after making parties in war.
        /// </summary>
        /// <param name="enemies party"></param>
        public void SetEnemies(WarParty party) {
            Enemies = party;
            JoinParty(major);
        }

        public bool ContainsCountry(CountryInfo country) {
            return countries.ContainsKey(country);
        }
        public bool ContainsCountry(WarCountryInfo country) {
            return countries.ContainsValue(country);
        }
        public void JoinParty(CountryInfo country) {
            if (!ContainsCountry(country) && !Enemies.ContainsCountry(country)) {
                WarCountryInfo c = new WarCountryInfo(country, this);
                countries.Add(country, c);
                country.wars.Add(war, c);
                RemoveDiplomaticRelationsOnJoin(country);
                if (country == GameInfo.PlayerCountry) {
                    WarList.Singleton.AddWar(c);
                }

                if (WarWindow.Singleton.war == war) {
                    WarWindow.Singleton.SetWar(war);
                }
            }
        }
        public void JoinParty(WarCountryInfo country) {
            if (!ContainsCountry(country) && !Enemies.ContainsCountry(country)) {
                countries.Add(country.country, country);
                country.country.wars.Add(war, country);
                RemoveDiplomaticRelationsOnJoin(country.country);
                if (country.country == GameInfo.PlayerCountry) {
                    WarList.Singleton.AddWar(country);
                }

                if (WarWindow.Singleton.war == war) {
                    WarWindow.Singleton.SetWar(war);
                }
            }
        }
        public void LeaveParty(WarCountryInfo country, bool isWarEnd = false) {
            if (ContainsCountry(country)) {
                foreach (var item in new List<ProvinceInfo>(country.enemyOccupatedProvinces)) {
                    item.SetCountry(item.NationalCountry);
                    country.RemoveEnemyOccupatedProvince(item);
                }

                foreach (var item in new List<ProvinceInfo>(country.localOccupatedProvinces)) {
                    item.SetCountry(item.NationalCountry);
                    item.Country.wars[war].RemoveEnemyOccupatedProvince(item);
                }

                if (country == major) {
                    foreach (var item in new Dictionary<CountryInfo, WarCountryInfo>(countries)) {
                        if (item.Value != major) {
                            LeaveParty(item.Value);
                        }
                    }

                    if (!isWarEnd) {
                        Enemies.LeaveParty(Enemies.major, true);
                    }
                }
                countries.Remove(country.country);
                country.country.wars.Remove(war);
                if (country.country == GameInfo.PlayerCountry) {
                    WarList.Singleton.RemoveWar(country);
                }

                if (WarWindow.Singleton.war == war) {
                    WarWindow.Singleton.SetWar(war);
                }

                if (!isWarEnd) {
                    if (countries.Count == 0) {
                        war.Delete();
                    }
                }

                foreach (var item in country.country.armies) {
                    if (item.Province.OccupationCounter.Army == item) {
                        item.Province.OccupationCounter.FindNewOccupant();
                    }
                }

                foreach (var item in GameInfo.provinces) {
                    item.Value.RefreshFogOfWar();
                }
            }
        }
        private void RemoveDiplomaticRelationsOnJoin(CountryInfo country) {
            foreach (var item in Enemies.countries) {
                CountryRelation r = country.relations[item.Key];
                for (int i = 0; i < r.relations.Length; i++) {
                    if (r.relations[i]) {
                        r.ChangeRelationState(i);
                    }
                }

                r.ChangePoints(-50);
            }
        }
    }
}
