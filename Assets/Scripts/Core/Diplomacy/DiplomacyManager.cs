using EuropeanWars.Core.Country;
using System.Collections.Generic;

namespace EuropeanWars.Core.Diplomacy {
    public static class DiplomacyManager {
        public static List<MilitaryAccess> militaryAccesses = new List<MilitaryAccess>();
        public static List<Alliance> alliances = new List<Alliance>();

        public static void AcceptRelation(DiplomaticRelation relation) {
            if (relation.GetType() == typeof(Alliance)) {
                Alliance.AcceptAlliance((Alliance)relation);
            }
            else if (relation.GetType() == typeof(MilitaryAccess)) {
                MilitaryAccess.AcceptAccess((MilitaryAccess)relation);
            }
        }
        public static void DeliceRelation(DiplomaticRelation relation) {
            if (relation.GetType() == typeof(Alliance)) {
                Alliance.DeliceAlliance((Alliance)relation);
            }
            else if (relation.GetType() == typeof(MilitaryAccess)) {
                MilitaryAccess.DeliceAccess((MilitaryAccess)relation);
            }
        }
        public static void DeleteRelation(DiplomaticRelation relation) {
            if (relation.GetType() == typeof(Alliance)) {
                Alliance.DeleteAlliance((Alliance)relation);
            }
            else if (relation.GetType() == typeof(MilitaryAccess)) {
                MilitaryAccess.DeleteAccess((MilitaryAccess)relation);
            }
        }

        public static void SetRelationsWithCountry(CountryInfo country, CountryInfo _country, int v) {
            if (country.relations.ContainsKey(_country) && _country.relations.ContainsKey(country)) {
                _country.relations[country] = v;
                country.relations[_country] = v;
            }
        }
    }
}
