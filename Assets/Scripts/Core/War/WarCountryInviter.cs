using EuropeanWars.Core.Country;
using System.Collections.Generic;

namespace EuropeanWars.Core.War {
    public class WarCountryInviter {
        private readonly WarInfo war;

        public WarCountryInviter(WarInfo war) {
            this.war = war;
        }

        private CountryInfo[] GetCountryFriends(CountryInfo country) {
            List<CountryInfo> result = new List<CountryInfo>();
            foreach (var item in country.alliances) {
                if (war.warReason.CanInviteCountryToWar(country, item.Key)) {
                    result.Add(item.Key);
                }
            }

            return result.ToArray();
        }
    }
}
