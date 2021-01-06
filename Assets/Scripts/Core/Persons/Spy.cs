using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Language;

namespace EuropeanWars.Core.Persons {
    public class Spy : Person {
        public int MonthlySpyNetworkIncrease { get; private set; }
        public CountryRelation CurrentlyBuildingSpyNetwork { get; private set; }
        public override string Speciality => LanguageDictionary.language["Spy"];
        public override string MoreInfo => LanguageDictionary.language["MonthlySpyNetworkIncrease"] + ": " + MonthlySpyNetworkIncrease;

        public Spy(string name, int birthYear, int deathYear, CountryInfo country, int monthlySpyNetworkIncrease) : base(name, birthYear, deathYear, country) {
            MonthlySpyNetworkIncrease = monthlySpyNetworkIncrease;
        }

        public void BuildSpyNetwork(CountryRelation relation) {
            if (CurrentlyBuildingSpyNetwork != relation) {
                if (CurrentlyBuildingSpyNetwork != null) {
                    CurrentlyBuildingSpyNetwork.monthlyPointsIncreaseChance -= MonthlySpyNetworkIncrease;
                }
                Spy spy = country.GetSpyInRelation(relation);
                if (spy != null) {
                    spy.BuildSpyNetwork(null);
                }

                CurrentlyBuildingSpyNetwork = relation;
                if (relation != null) {
                    CurrentlyBuildingSpyNetwork.monthlyPointsIncreaseChance += MonthlySpyNetworkIncrease;
                }
            }
        }
    }
}
