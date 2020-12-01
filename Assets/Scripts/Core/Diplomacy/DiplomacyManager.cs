using EuropeanWars.Core.Country;
using EuropeanWars.Core.War;
using System.Collections.Generic;

namespace EuropeanWars.Core.Diplomacy {
    public static class DiplomacyManager {
        public static Dictionary<int, WarInfo> wars = new Dictionary<int, WarInfo>();

        public static void DeclareWar(WarReason warReason, CountryInfo attacker, CountryInfo defender) {
            WarInfo war = new WarInfo(warReason, attacker, defender);
            wars.Add(war.id, war);
        }
    }
}
