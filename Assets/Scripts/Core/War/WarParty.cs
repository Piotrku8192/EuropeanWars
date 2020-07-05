using EuropeanWars.Core.Country;
using System.Collections.Generic;
using System.Linq;

namespace EuropeanWars.Core.War {
    public class WarParty {
        public readonly WarInfo war;
        public WarParty Enemies { get; private set; }
        public readonly WarCountryInfo major;
        public readonly List<WarCountryInfo> countries = new List<WarCountryInfo>();
        public WarScore WarScore { get; private set; }

        /// <summary>
        /// After making parties you must set Enemies party by invoking SetEnemies method.
        /// </summary>
        /// <param name="war"></param>
        /// <param name="major"></param>
        public WarParty(WarInfo war, CountryInfo major) {
            this.war = war;
            this.major = new WarCountryInfo(major, this);
            countries = new List<WarCountryInfo>();
        }

        /// <summary>
        /// Must be setted after making parties in war.
        /// </summary>
        /// <param name="enemies party"></param>
        public void SetEnemies(WarParty party) {
            Enemies = party;
        }

        public bool ContainsCountry(CountryInfo country) {
            return major.country == country || countries.Where(t => t.country == country).Any();
        }
        public bool ContainsCountry(WarCountryInfo country) {
            return major == country || countries.Contains(country);
        }
        public void JoinParty(CountryInfo country) {
            if (!ContainsCountry(country) && !Enemies.ContainsCountry(country)) {
                WarCountryInfo c = new WarCountryInfo(country, this);
                countries.Add(c);
                country.wars.Add(war, c);
                //TODO: Update WarScore in all countries in war
            }
        }
        public void LeaveParty(WarCountryInfo country) {
            if (ContainsCountry(country)) {
                if (country != major) {
                    countries.Remove(country);
                    country.country.wars.Remove(war);
                    //TODO: Update WarScore in all countries in war
                }
            }
        }
    }
}
