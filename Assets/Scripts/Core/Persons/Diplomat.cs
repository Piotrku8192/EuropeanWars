using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Language;

namespace EuropeanWars.Core.Persons {
    public class Diplomat : Person {
        /// <summary>
        /// Tells how much has to be added to monthlyPointsIncreaseChance when diplomat is improving relations
        /// </summary>
        public float ImproveRelationsModifier { get; private set; }
        public CountryRelation CurrentlyImprovingRelation { get; private set; }
        public override string Speciality => LanguageDictionary.language["Diplomat"];
        public override string MoreInfo => LanguageDictionary.language["ImproveRelationsModifier"] + ": " + ImproveRelationsModifier;

        public Diplomat(string name, int birthYear, int deathYear, float improveRelationsModifier) : base(name, birthYear, deathYear) {
            ImproveRelationsModifier = improveRelationsModifier;
        }

        public void ImproveRelation(CountryRelation relation) {
            if (CurrentlyImprovingRelation != relation) {
                if (CurrentlyImprovingRelation != null) {
                    CurrentlyImprovingRelation.monthlyPointsIncreaseChance -= ImproveRelationsModifier;
                }
                CurrentlyImprovingRelation = relation;
                CurrentlyImprovingRelation.monthlyPointsIncreaseChance += ImproveRelationsModifier;
            }
        }
    }
}
