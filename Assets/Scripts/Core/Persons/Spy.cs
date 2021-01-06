using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Language;

namespace EuropeanWars.Core.Persons {
    public class Spy : Person {
        public int MonthlySpyNetworkChange { get; private set; }
        public CountryRelation CurrentlyBuildingSpyNetwork { get; private set; }
        public override string Speciality => LanguageDictionary.language["Spy"];
        public override string MoreInfo => LanguageDictionary.language["MonthlySpyNetworkIncrease"] + ": " + MonthlySpyNetworkChange;

        public Spy(string name, int birthYear, int deathYear, CountryInfo country, int monthlySpyNetworkIncrease) : base(name, birthYear, deathYear, country) {
            MonthlySpyNetworkChange = monthlySpyNetworkIncrease;
        }

        public void BuildSpyNetwork(CountryRelation relation) {
            if (CurrentlyBuildingSpyNetwork != relation) {
                if (CurrentlyBuildingSpyNetwork != null) {
                    CurrentlyBuildingSpyNetwork.monthlySpyNetworkChange -= MonthlySpyNetworkChange;
                }
                Spy spy = country.GetSpyInRelation(relation);
                if (spy != null) {
                    spy.BuildSpyNetwork(null);
                }

                CurrentlyBuildingSpyNetwork = relation;
                if (relation != null) {
                    CurrentlyBuildingSpyNetwork.monthlySpyNetworkChange += MonthlySpyNetworkChange;
                }
            }
        }
    }
}
