using EuropeanWars.Core.Country;
using System;

namespace EuropeanWars.Core.Diplomacy {
    public enum DiplomaticRelation {
        Alliance = 0,
        MilitaryAccess = 1,
        TradeAgreament = 2,
        RoyalMariage = 3,
        Truce = 4,
    }

    public enum DiplomaticAction {
        Insult = 0,
        Vassalization = 1
    }

    public class CountryRelation {
        public int points;
        public bool[] relations;

        public CountryRelation(int points) {
            this.points = points;
            this.relations = new bool[Enum.GetValues(typeof(DiplomaticRelation)).Length];
        }

        public void ChangeRelationState(DiplomaticRelation relation, CountryInfo sender, CountryInfo receiver) {
            if (!sender.isPlayer && !receiver.isPlayer) {
                if (GameInfo.countryAIs[receiver].IsDiplomaticRelationChangeAccepted(relation, sender)) {
                    ChangeRelationState(relation);
                }
            }
            else if (sender.isPlayer && !receiver.isPlayer) {
                if (GameInfo.countryAIs[receiver].IsDiplomaticRelationChangeAccepted(relation, sender)) {
                    //Send to all players that they must change relation state.
                }
            }
            else if (!sender.isPlayer && receiver.isPlayer) {
                if (GameInfo.PlayerCountry == receiver) {
                    //Show communicate to player, if accepted send to all
                }
            }
            else {
                //Send request to other player and if he accepted send to all as in situatuion above
            }
        }

        void ChangeRelationState(DiplomaticRelation relation) {
            //TODO: Add switch and additional actions in this place
            relations[(int)relation] = !relations[(int)relation];
        }
    }
}
