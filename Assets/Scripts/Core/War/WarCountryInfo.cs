using EuropeanWars.Core.Country;

namespace EuropeanWars.Core.War {
    public class WarCountryInfo {
        public readonly CountryInfo country;
        public readonly WarParty party;
        public WarScore WarScore { get; private set; }

        public WarCountryInfo(CountryInfo country, WarParty party) {
            this.country = country;
            this.party = party;
        }
    }
}
