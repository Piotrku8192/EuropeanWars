using Boo.Lang;
using EuropeanWars.Core.Country;

namespace EuropeanWars.Core.War {
    public class WarInfo {
        private static int nextId = 0;

        public readonly int id;
        public readonly WarReason warReason;
        
        public readonly WarParty attackers;
        public readonly WarParty defenders;

        public WarInfo(WarReason warReason, CountryInfo attacker, CountryInfo defender) {
            id = nextId;
            nextId++;
            this.warReason = warReason;
            attackers = new WarParty(this, attacker);
            defenders = new WarParty(this, defender);
            attackers.SetEnemies(defenders);
            defenders.SetEnemies(attackers);
            WarCountryInviter inviter = new WarCountryInviter(this);
            inviter.InviteFriends();
        }

        public bool ContainsCountry(CountryInfo country) {
            return attackers.ContainsCountry(country);
        }

        public void JoinWar(CountryInfo country, bool isAttacker) {
            if (isAttacker) {
                attackers.JoinParty(country);
            }
            else {
                defenders.JoinParty(country);
            }
        }
    }
}
