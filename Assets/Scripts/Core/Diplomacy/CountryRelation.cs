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

        public void TryChangeRelationState(DiplomaticRelation relation, CountryInfo sender, CountryInfo receiver) {
            if (relations[(int)relation]) {
                if (!sender.isPlayer) {
                    ChangeRelationState(relation);
                }
                else {
                    SendMessage(relation, sender, receiver, 1039);
                }

                return;
            }

            if (!sender.IsInWarAgainstCountry(receiver)) {
                if (!sender.isPlayer && !receiver.isPlayer) {
                    if (GameInfo.countryAIs[receiver].IsDiplomaticRelationChangeAccepted(relation, sender)) {
                        ChangeRelationState(relation);
                    }
                }
                else if (sender.isPlayer && !receiver.isPlayer) {
                    if (GameInfo.countryAIs[receiver].IsDiplomaticRelationChangeAccepted(relation, sender)) {
                        SendMessage(relation, sender, receiver, 1039);
                    }
                }
                else if (!sender.isPlayer && receiver.isPlayer) {
                    if (GameInfo.PlayerCountry == receiver) {
                        ShowRequest(relation, sender, receiver);
                    }
                }
                else if (GameInfo.PlayerCountry == sender) {
                    SendMessage(relation, sender, receiver, 1040);
                }
            }
        }
        public void ProcessRequest(DiplomaticRelation relation, CountryInfo sender, CountryInfo receiver) {
            if (GameInfo.PlayerCountry == receiver) {
                ShowRequest(relation, sender, receiver);
            }
        }
        private void ShowRequest(DiplomaticRelation relation, CountryInfo sender, CountryInfo receiver) {
            DipRequestWindow window = DiplomacyWindow.Singleton.SpawnRequest(sender, receiver, true);

            //TODO: translations!!!
            window.acceptText.text = "Zaakceptuj";
            window.deliceText.text = "Odrzuć";
            window.title.text = "" + relation.ToString();
            string description = "Zapytanie xD";
            window.description.text = description;

            NetOutgoingMessage acceptMessage = Client.Singleton.c.CreateMessage();
            acceptMessage.Write((ushort)1039);
            acceptMessage.Write(sender.id);
            acceptMessage.Write(receiver.id);
            acceptMessage.Write((int)relation);
            window.acceptMessage = acceptMessage;

            NetOutgoingMessage deliceMessage = Client.Singleton.c.CreateMessage();
            deliceMessage.Write((ushort)1041);
            deliceMessage.Write(sender.id);
            deliceMessage.Write(receiver.id);
            deliceMessage.Write((int)relation);
            window.deliceMessage = deliceMessage;
        }
        private void SendMessage(DiplomaticRelation relation, CountryInfo sender, CountryInfo receiver, ushort id) {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write(id);
            msg.Write(sender.id);
            msg.Write(receiver.id);
            msg.Write((int)relation);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
