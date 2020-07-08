using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.UI.Windows;
using System.Collections.Generic;
using System.Linq;

namespace EuropeanWars.Core.War {
    public class WarParty {
        public readonly WarInfo war;
        public WarParty Enemies { get; private set; }
        public readonly WarCountryInfo major;
        public readonly Dictionary<CountryInfo, WarCountryInfo> countries;

        public int PartyScoreCost => countries.Sum(t => t.Value.CountryScoreCost);
        public int WarScore => countries.Sum(t => t.Value.WarScore);
        public float PercentWarScore => WarScore / Enemies.PartyScoreCost;

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
            }
        }
        public void LeaveParty(WarCountryInfo country) {
            if (ContainsCountry(country)) {
                if (country == major) {
                    foreach (var item in countries) {
                        if (item.Value != major) {
                            LeaveParty(item.Value);
                        }
                    }
                }
                countries.Remove(country.country);
                country.country.wars.Remove(war);
                if (country.country == GameInfo.PlayerCountry) {
                    WarList.Singleton.RemoveWar(country);
                }
            }
        }
        private void RemoveDiplomaticRelationsOnJoin(CountryInfo country) {
            foreach (var item in Enemies.countries) {
                if (country.alliances.ContainsKey(item.Key)) {
                    Alliance.DeleteAllianceClient(country.alliances[item.Key], item.Key.id);
                }
                if (country.militaryAccesses.ContainsKey(item.Key)) {
                    MilitaryAccess.DeleteAccessClient(country.militaryAccesses[item.Key], item.Key.id);
                }
            }
        }
    }
}
