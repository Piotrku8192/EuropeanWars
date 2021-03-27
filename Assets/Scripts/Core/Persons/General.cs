using EuropeanWars.Core.Army;
using EuropeanWars.Core.Country;
using UnityEngine;

namespace EuropeanWars.Core.Persons {
    public class General : Person {
        public float[] attackModifiers;
        public ArmyInfo army;

        public General(string firstName, string lastName, int birthYear, int deathYear, CountryInfo country, Sprite portrait) : base(firstName, lastName, birthYear, deathYear, country, portrait) {
        }
    }
}
