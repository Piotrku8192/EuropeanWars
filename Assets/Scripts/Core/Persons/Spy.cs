using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Language;
using UnityEngine;

namespace EuropeanWars.Core.Persons {
    public class Spy : Person {
        public int MonthlySpyNetworkChange { get; private set; }
        public CountryRelation CurrentlyBuildingSpyNetwork { get; private set; }
        public override string Speciality => LanguageDictionary.language["Spy"];
        public override string MoreInfo => LanguageDictionary.language["MonthlySpyNetworkIncrease"] + ": " + MonthlySpyNetworkChange;

        public Spy(string firstName, string lastName, int birthYear, int deathYear, CountryInfo country, Sprite portrait, int monthlySpyNetworkIncrease) : base(firstName, lastName, birthYear, deathYear, country, portrait) {
            MonthlySpyNetworkChange = monthlySpyNetworkIncrease;
        }

        public void BuildSpyNetwork(CountryRelation relation) {
            if (CurrentlyBuildingSpyNetwork != relation) {
                Spy spy = country.GetSpyInRelation(relation);
                if (spy != null) {
                    spy.BuildSpyNetwork(null);
                }

                CurrentlyBuildingSpyNetwork = relation;
            }
        }
    }
}
