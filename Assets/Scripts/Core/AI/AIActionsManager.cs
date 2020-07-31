using EuropeanWars.Core.Country;

namespace EuropeanWars.Core.AI {
    public class AIActionsManager {
        private readonly CountryInfo country;

        public AIActionsManager(CountryInfo country) {
            this.country = country;
        }
    }
}