using EuropeanWars.Core.Country;
using EuropeanWars.Core.War;
using System.Collections.Generic;

namespace EuropeanWars.Core.Diplomacy_Old {
    public static class DiplomacyManager {
        public static Dictionary<int, WarInfo> wars = new Dictionary<int, WarInfo>();
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

        public static void DeclareWar(WarReason warReason, CountryInfo attacker, CountryInfo defender) {
            WarInfo war = new WarInfo(warReason, attacker, defender);
            wars.Add(war.id, war);
        }
    }
}
