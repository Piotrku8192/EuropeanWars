using EuropeanWars.Core.Country;
using EuropeanWars.Network;
using Lidgren.Network;
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

        public void TryChangeRelationState(DiplomaticRelation relation, CountryInfo sender, CountryInfo receiver) {
            if (!sender.isPlayer && !receiver.isPlayer) {
                if (GameInfo.countryAIs[receiver].IsDiplomaticRelationChangeAccepted(relation, sender)) {
                    ChangeRelationState(relation);
                }
            }
            else if (sender.isPlayer && !receiver.isPlayer) {
                if (GameInfo.countryAIs[receiver].IsDiplomaticRelationChangeAccepted(relation, sender)) {
                    NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                    msg.Write((ushort)1039);
                    msg.Write(sender.id);
                    msg.Write(receiver.id);
                    msg.Write((int)relation);
                    Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                }
            }
            else if (!sender.isPlayer && receiver.isPlayer) {
                if (GameInfo.PlayerCountry == receiver) {
                    //Show communicate to player, if accepted send to all
                }
            }
            else {
                if (GameInfo.PlayerCountry == receiver) {
                    //Show communicate to player, if accepted send to all
                }
                else {
                    NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                    msg.Write((ushort)1040);
                    msg.Write(sender.id);
                    msg.Write(receiver.id);
                    msg.Write((int)relation);
                    Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                }
            }
        }

        public void ChangeRelationState(DiplomaticRelation relation) {
            //TODO: Add switch and additional actions in this place
            relations[(int)relation] = !relations[(int)relation];
        }
    }
}
