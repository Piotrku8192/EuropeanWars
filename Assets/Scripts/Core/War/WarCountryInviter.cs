﻿using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EuropeanWars.Core.War {
    public class WarCountryInviter {
        private readonly WarInfo war;

        public WarCountryInviter(WarInfo war) {
            this.war = war;
        }

        public void InviteFriends() {
            CountryInfo[] defenders = GetCountryFriends(war.defenders.major.country);
            CountryInfo[] attackers = GetCountryFriends(war.attackers.major.country).Where(t => !defenders.Contains(t)).ToArray();

            foreach (var item in defenders) {
                SendInvitation(war.defenders.major.country, item, false);
            }
            foreach (var item in attackers) {
                SendInvitation(war.attackers.major.country, item, true);
            }
        }

        private void SendInvitation(CountryInfo inviter, CountryInfo country, bool isAttacker) {
            if (country == GameInfo.PlayerCountry) {
                DiplomacyWindow.Singleton.SpawnWarInvitation(war, inviter, isAttacker);
            }
            else {
                //TODO: Invoke bot decision and add if bot agrees
            }
        }

        private CountryInfo[] GetCountryFriends(CountryInfo country) {
            List<CountryInfo> result = new List<CountryInfo>();
            foreach (var item in country.alliances) {
                if (war.warReason.CanInviteCountryToWar(country, item.Key)) {
                    result.Add(item.Key);
                }
            }

            return result.ToArray();
        }
    }
}