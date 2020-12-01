using EuropeanWars.Core.Country;
using EuropeanWars.GameMap;
using EuropeanWars.Network;
using EuropeanWars.UI.Windows;
using Lidgren.Network;
using System;
using UnityEngine;

namespace EuropeanWars.Core.Diplomacy {
    public enum DiplomaticRelation {
        Alliance = 0,
        RoyalMariage = 1,
        MilitaryAccess = 2,
        TradeAgreament = 3,
    }

    public enum DiplomaticAction {
        Insult = 0,
        Vassalization = 1
    }

    public class CountryRelation {
        public int Points { get; private set; }
        public bool[] relations;
        public ushort truceInMonths;

        public bool withPlayerCountry;

        public CountryRelation(int points) {
            Points = Mathf.Clamp(points, -100, 100);
            this.relations = new bool[Enum.GetValues(typeof(DiplomaticRelation)).Length];
            truceInMonths = 0;
        }


        public bool CanChangeRelationStateTo(DiplomaticRelation relation, bool targetState) => relations[(int)relation] != targetState;

        public void TryChangeRelationState(DiplomaticRelation relation, CountryInfo sender, CountryInfo receiver) {
            if (!sender.IsInWarAgainstCountry(receiver)) {
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
        }

        public void ChangeRelationState(DiplomaticRelation relation) {
            ChangeRelationState((int)relation);
        }

        public void ChangeRelationState(int relation) {
            //TODO: Add switch and additional actions in this place
            relations[relation] = !relations[relation];
            if (DiplomacyWindow.Singleton.window.activeInHierarchy) {
                DiplomacyWindow.Singleton.UpdateWindow();
            }

            if (withPlayerCountry) {
                foreach (var item in GameInfo.provinces) {
                    item.Value.RefreshFogOfWar();
                }
            }
        }

        public void ChangePoints(int change) {
            Points = Mathf.Clamp(Points + change, -100, 100);
        }
    }
}
