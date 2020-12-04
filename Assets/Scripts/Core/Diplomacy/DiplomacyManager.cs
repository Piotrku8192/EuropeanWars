using EuropeanWars.Core.Country;
using EuropeanWars.Core.War;
using EuropeanWars.UI.Windows;
using System.Collections.Generic;

namespace EuropeanWars.Core.Diplomacy {
    public static class DiplomacyManager {
        public static Dictionary<int, WarInfo> wars = new Dictionary<int, WarInfo>();

        public static void DeclareWar(WarReason warReason, CountryInfo attacker, CountryInfo defender) {
            if (attacker == GameInfo.PlayerCountry) {
                DiplomacyWindow.Singleton.UpdateWindow();
            }

            if (!attacker.IsInWarAgainstCountry(defender)) {
                if (defender == GameInfo.PlayerCountry) {
                    //TODO: Implement translation
                    DipRequestWindow win = DiplomacyWindow.Singleton.SpawnRequest(attacker, defender, true);
                    win.title.text = "Wojna!";
                    win.description.text = "Nasz niedaleki sąsiad wypowiedział nam wojnę!";
                    win.acceptText.text = "Ok";
                    win.deliceText.text = "Ok";
                }
                WarInfo war = new WarInfo(warReason, attacker, defender);
                wars.Add(war.id, war);
            }
        }
    }
}
