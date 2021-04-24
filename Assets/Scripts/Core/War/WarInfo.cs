using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Time;
using EuropeanWars.UI.Windows;

namespace EuropeanWars.Core.War {
    public class WarInfo {
        private static int nextId = 0;

        public readonly int id;
        public readonly WarReason warReason;

        public readonly int startYear;
        public readonly int startMonth;
        public readonly int startDay;

        public readonly WarParty attackers;
        public readonly WarParty defenders;

        public WarInfo(WarReason warReason, CountryInfo attacker, CountryInfo defender) {
            id = nextId;
            startDay = TimeManager.day;
            startMonth = TimeManager.month;
            startYear = TimeManager.year;
            nextId++;
            this.warReason = warReason;
            attackers = new WarParty(this, attacker);
            defenders = new WarParty(this, defender);
            attackers.SetEnemies(defenders);
            defenders.SetEnemies(attackers);
            WarCountryInviter inviter = new WarCountryInviter(this);
            inviter.InviteFriends();
        }

        public void Delete() {
            DiplomacyManager.wars.Remove(id);
            if (WarWindow.Singleton.war == this) {
                WarWindow.Singleton.war = null;
                WarWindow.Singleton.windowObject.SetActive(false);
            }
        }

        public bool ContainsCountry(CountryInfo country) {
            return attackers.ContainsCountry(country) || defenders.ContainsCountry(country);
        }

        public void JoinWar(CountryInfo country, bool isAttacker) {
            if (isAttacker) {
                attackers.JoinParty(country);
            }
            else {
                defenders.JoinParty(country);
            }
            foreach (var item in country.vassals) {
                JoinWar(item, isAttacker);
            }
        }
    }
}
