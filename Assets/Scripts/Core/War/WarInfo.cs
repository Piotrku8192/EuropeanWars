using Boo.Lang;
using EuropeanWars.Core.Country;

namespace EuropeanWars.Core.War {
    public class WarInfo {
        public readonly WarReason warReason;
        
        public readonly WarParty attackers;
        public readonly WarParty defenders;

        public WarInfo(WarReason warReason, CountryInfo attacker, CountryInfo defender) {
            this.warReason = warReason;
            attackers = new WarParty(this, attacker);
            defenders = new WarParty(this, defender);
            attackers.SetEnemies(defenders);
            defenders.SetEnemies(attackers);
            WarCountryInviter inviter = new WarCountryInviter(this);
            inviter.InviteFriends();
        }
    }
}
