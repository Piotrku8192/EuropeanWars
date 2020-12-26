using EuropeanWars.Core.Language;

namespace EuropeanWars.Core.Persons {
    public class Spy : Person {
        public int MonthlySpyNetworkIncrease { get; private set; }
        public override string Speciality => LanguageDictionary.language["Spy"];
        public override string MoreInfo => LanguageDictionary.language["MonthlySpyNetworkIncrease"] + ": " + MonthlySpyNetworkIncrease;

        public Spy(string name, int birthYear, int deathYear, int monthlySpyNetworkIncrease) : base(name, birthYear, deathYear) {
            MonthlySpyNetworkIncrease = monthlySpyNetworkIncrease;
        }
    }
}
