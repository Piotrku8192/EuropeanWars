using EuropeanWars.Core.Country;
using EuropeanWars.UI.Windows;
using System.Collections.Generic;
using System.Linq;

namespace EuropeanWars.Core.Diplomacy {
    public static class DiplomacyManager {
        public static List<Alliance> alliances = new List<Alliance>();

        public static void CreateRelation(DiplomaticRelation relation) {
            if (relation.GetType() == typeof(Alliance)) {
                CreateAlliance((Alliance)relation);
            }
        }
        public static void DeleteRelation(DiplomaticRelation relation) {
            if (relation.GetType() == typeof(Alliance)) {
                DeleteAlliance((Alliance)relation);
            }
        }

        public static void RelationRequest(CountryInfo sender, CountryInfo receiver) {
            Alliance a = new Alliance();
            a.countries.Add(sender);
            a.countries.Add(receiver);

            if (!receiver.isPlayer) {
                //TODO: Add bot request mechanic.
            }
            else if (receiver == GameInfo.PlayerCountry) {
                DiplomacyWindow.Singleton.SpawnRequest(a);
            }
        }

        public static void CreateAlliance(Alliance alliance) {
            if (alliance.countries.Count == 2) {
                alliances.Add(alliance);
                alliance.countries[0].alliances.Add(alliance.countries[1], alliance);
                alliance.countries[1].alliances.Add(alliance.countries[0], alliance);
            }
        }
        public static void DeleteAlliance(Alliance alliance) {
            foreach (var item in alliance.countries) {
                item.alliances.Remove(item.alliances.Where(t => t.Value == alliance).FirstOrDefault().Key);
            }
            alliances.Remove(alliance);
        }
    }
}
