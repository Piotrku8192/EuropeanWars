using EuropeanWars.Core.Country;

namespace EuropeanWars.Core.Persons {
    public class General : Person {
        public General(string name, int birthYear, int deathYear, CountryInfo country) : base(name, birthYear, deathYear, country) {
        }
    }
}
