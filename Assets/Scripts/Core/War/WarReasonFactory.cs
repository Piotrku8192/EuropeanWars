using EuropeanWars.Core.Country;
using System.Collections.Generic;

namespace EuropeanWars.Core.War {
    public class WarReasonFactory {
        public readonly CountryInfo attacker;
        public readonly CountryInfo defender;

        public WarReasonFactory(CountryInfo attacker, CountryInfo defender) {
            this.attacker = attacker;
            this.defender = defender;
        }

        public WarReason[] GetReasons() {
            List<WarReason> result = new List<WarReason>();

            foreach (var item in attacker.claimedProvinces) {
                if (item.NationalCountry == defender) {
                    result.Add(new ConquestWarReason(item));
                }
            }

            //TODO: Add other reasons

            return result.ToArray();
        }
    }
}
