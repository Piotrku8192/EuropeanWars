using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.UI.Windows;
using System.Collections.Generic;
using System.Linq;

namespace EuropeanWars.Core.War {
    public class WarCountryInviter {
        private readonly WarInfo war;

        public WarCountryInviter(WarInfo war) {
            this.war = war;
        }

        public void InviteFriends() {
            CountryInfo[] defenders = GetCountryFriends(war.defenders.major.country, war.attackers);
            CountryInfo[] attackers = GetCountryFriends(war.attackers.major.country, war.defenders).Where(t => !defenders.Contains(t)).ToArray();

            List<CountryInfo> defs = new List<CountryInfo>();
            List<CountryInfo> attacks = new List<CountryInfo>();

            foreach (var d in defenders) {
                if (!defenders.Where(t => d.IsInWarAgainstCountry(t)).Any()
                    && !attackers.Where(t => d.IsInWarAgainstCountry(t)).Any()) {
                    defs.Add(d);
                }
            }

            foreach (var a in attackers) {
                if (!attackers.Where(t => a.IsInWarAgainstCountry(t)).Any()
                    && !defenders.Where(t => a.IsInWarAgainstCountry(t)).Any()) {
                    attacks.Add(a);
                }
            }

            foreach (var item in defs) {
                SendInvitation(war.defenders.major.country, item, false);
            }
            foreach (var item in attacks) {
                SendInvitation(war.attackers.major.country, item, true);
            }
        }

        private void SendInvitation(CountryInfo inviter, CountryInfo country, bool isAttacker) {
            if (country == GameInfo.PlayerCountry) {
                DiplomacyWindow.Singleton.SpawnWarInvitation(war, inviter, isAttacker);
            }
            else if (!country.isPlayer) {
                if (GameInfo.random.Next(0, 10) > 4) {//TODO: Invoke bot decision and add if bot agrees
                    war.JoinWar(country, isAttacker);
                }
                else if (country.relations[inviter].relations[(int)DiplomaticRelation.Alliance] && !isAttacker) {
                    country.relations[inviter].ChangeRelationState(DiplomaticRelation.Alliance, inviter, country);
                }
            }
        }

        private CountryInfo[] GetCountryFriends(CountryInfo country, WarParty enemies) {
            List<CountryInfo> result = new List<CountryInfo>();
            foreach (var item in country.relations) {
                if (item.Value.relations[(int)DiplomaticRelation.Alliance]) {
                    if (!item.Key.IsInWarAgainstCountry(country) && !item.Key.IsInWarAgainstCountry(enemies.major.country)) {
                        if (war.warReason.CanInviteCountryToWar(country, item.Key)) {
                            result.Add(item.Key);
                        }
                    }
                }
            }

            return result.ToArray();
        }
    }
}
