using EuropeanWars.Core.Country;
using UnityEngine;

namespace EuropeanWars.Core.Persons {
    public class General : Person {
        public General(string name, int birthYear, int deathYear, CountryInfo country, Sprite portrait) : base(name, birthYear, deathYear, country, portrait) {
        }
    }
}
