using EuropeanWars.Core.Army;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Language;
using UnityEngine;

namespace EuropeanWars.Core.Persons {
    public class General : Person {
        public float[] attackModifiers;
        public ArmyInfo army;

        public override string Speciality => LanguageDictionary.language["General"];
        public override string MoreInfo => LanguageDictionary.language["Infantry"] + ": " + attackModifiers[0] * 100 + "%  " +
            LanguageDictionary.language["Cavalry"] + ": " + attackModifiers[1] * 100 + "%\n" +
            LanguageDictionary.language["Artillery"] + ": " + attackModifiers[2] * 100 + "%  " +
            LanguageDictionary.language["Tabor"] + ": " + attackModifiers[3] * 100 + "%\n" +
            LanguageDictionary.language["Navy"] + ": " + attackModifiers[4] * 100 + "%\n";

        public General(string firstName, string lastName, int birthYear, int deathYear, CountryInfo country, Sprite portrait, float[] attackModifiers) : base(firstName, lastName, birthYear, deathYear, country, portrait) {
            this.attackModifiers = attackModifiers;
        }
    }
}
