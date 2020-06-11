using System.Collections.Generic;

namespace EuropeanWars.Core.Diplomacy {
    public static class DiplomacyManager {
        public static List<Alliance> alliances = new List<Alliance>();
        
        public static void AcceptRelation(DiplomaticRelation relation) {
            if (relation.GetType() == typeof(Alliance)) {
                Alliance.AcceptAlliance((Alliance)relation);
            }
        }
        public static void DeliceRelation(DiplomaticRelation relation) {
            if (relation.GetType() == typeof(Alliance)) {
                Alliance.DeliceAlliance((Alliance)relation);
            }
        }
        public static void DeleteRelation(DiplomaticRelation relation) {
            if (relation.GetType() == typeof(Alliance)) {
                Alliance.DeleteAlliance((Alliance)relation);
            }
        }
    }
}
