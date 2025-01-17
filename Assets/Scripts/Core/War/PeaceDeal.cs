﻿using EuropeanWars.Core.Language;
using EuropeanWars.Core.Time;
using EuropeanWars.Network;
using EuropeanWars.UI.Windows;
using Lidgren.Network;
using System.Collections.Generic;
using UnityEngine;

namespace EuropeanWars.Core.War {
    public class PeaceDeal {
        public readonly WarInfo war;
        public readonly WarCountryInfo sender;
        public readonly WarCountryInfo receiver;

        public int nextElementId;

        public Dictionary<int, PeaceDealElement> senderElements = new Dictionary<int, PeaceDealElement>();
        public Dictionary<int, PeaceDealElement> receiverElements = new Dictionary<int, PeaceDealElement>();

        public List<int> selectedSenderElements = new List<int>();
        public List<int> selectedReceiverElements = new List<int>();

        public int GainedGold { get; private set; }
        public int UsedWarScore { get; private set; }

        public int SenderWarScore => sender.IsMajor ? (receiver.IsMajor ? sender.party.PercentWarScore : -receiver.PercentWarScore) : sender.PercentWarScore;
        public int ReceiverWarScore => receiver.IsMajor ? (sender.IsMajor ? receiver.party.PercentWarScore : -sender.PercentWarScore) : receiver.PercentWarScore;

        public PeaceDeal(WarInfo war, WarCountryInfo sender, WarCountryInfo receiver) {
            this.war = war;
            this.sender = sender;
            this.receiver = receiver;
            PeaceDealElementsFactory factory = new PeaceDealElementsFactory(this);
            factory.FillPeaceDeal();
        }

        public static bool CanMakePeaceDeal(WarInfo war, WarCountryInfo sender, WarCountryInfo receiver) {
            return TimeManager.year - war.startYear > 1 
                && ((!sender.country.isVassal || (sender.country.sovereign && !sender.party.ContainsCountry(sender.country.suzerain))) || sender.IsMajor)
                && ((!receiver.country.isVassal || (receiver.country.sovereign && !receiver.party.ContainsCountry(receiver.country.suzerain))) || receiver.IsMajor);
        }

        /// <summary>
        /// Adds gainedGold by change * 10. Every 1 change costs 1% warScore.
        /// </summary>
        /// <param name="change">GainedGold change in 10 size bits.</param>
        public void ChangeGold(int change) {
            change = Mathf.Clamp(change, -GainedGold / 10, receiver.country.gold < 0 ? 0 : (receiver.country.gold - GainedGold) / 10);
            GainedGold += change * 10;
            UsedWarScore += change;
        }

        public void SelectSenderElement(PeaceDealElement element) {
            if (element != null && !selectedSenderElements.Contains(element.id) && element.CanBeSelected(this)) {
                selectedSenderElements.Add(element.id);
                UsedWarScore += element.WarScoreCost;
            }
        }

        public void UnselectSenderElement(PeaceDealElement element) {
            if (selectedSenderElements.Remove(element.id)) {
                UsedWarScore -= element.WarScoreCost;
            }
        }

        public void SelectReceiverElement(PeaceDealElement element) {
            if (element != null && !selectedReceiverElements.Contains(element.id) && element.CanBeSelected(this)) {
                selectedReceiverElements.Add(element.id);
                UsedWarScore -= element.WarScoreCost;
            }
        }

        public void UnselectReceiverElement(PeaceDealElement element) {
            if (selectedReceiverElements.Remove(element.id)) {
                UsedWarScore += element.WarScoreCost;
            }
        }

        public void Send() {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)1036);
            msg.Write(war.id);
            msg.Write(sender.country.id);
            msg.Write(receiver.country.id);
            msg.Write(GainedGold);
            msg.Write(selectedSenderElements.Count);
            foreach (var item in selectedSenderElements) {
                msg.Write(item);
            }
            msg.Write(selectedReceiverElements.Count);
            foreach (var item in selectedReceiverElements) {
                msg.Write(item);
            }

            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendDelice() {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)1038);
            msg.Write(sender.country.id);
            msg.Write(receiver.country.id);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendRequest() {
            if (receiver.country.isPlayer) {
                NetworkSendRequest();
            }
            else {
                ProcessRequest();
            }
        }

        private void NetworkSendRequest() {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)1037);
            msg.Write(receiver.country.id);
            msg.Write(war.id);
            msg.Write(sender.country.id);
            msg.Write(receiver.country.id);
            msg.Write(GainedGold);
            msg.Write(selectedSenderElements.Count);
            foreach (var item in selectedSenderElements) {
                msg.Write(item);
            }
            msg.Write(selectedReceiverElements.Count);
            foreach (var item in selectedReceiverElements) {
                msg.Write(item);
            }

            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        public void ProcessRequest() {
            if (receiver.country == GameInfo.PlayerCountry && SenderWarScore < 99) {
                DipRequestWindow window = DiplomacyWindow.Singleton.SpawnRequest(sender.country, receiver.country, true);

                window.acceptText.text = LanguageDictionary.language["Accept"];
                window.deliceText.text = LanguageDictionary.language["Delice"];
                window.title.text = LanguageDictionary.language["PeaceDeal"];
                string description = GainedGold == 0 ? "" : string.Format(LanguageDictionary.language["PeaceDealGoldDescription"], receiver.country.Name, GainedGold);
                foreach (var item in selectedSenderElements) {
                    description += senderElements[item].Name + "\n";
                }
                foreach (var item in selectedReceiverElements) {
                    description += receiverElements[item].Name;
                }
                window.description.text = description;

                window.onAccept = Send;
                window.onDelice = SendDelice;
            }
            else {
                //TODO: Make bot decision.
                //TODO: Remove this temporary code when bots are done.
                if (UsedWarScore <= SenderWarScore) {
                    Send();
                }
                else {
                    //TODO: Send delice message
                }
            }
        }

        public void Execute() {
            if (war.ContainsCountry(GameInfo.PlayerCountry)) {
                DipRequestWindow window = DiplomacyWindow.Singleton.SpawnRequest(sender.country, receiver.country, true);

                window.acceptText.text = "Ok";
                window.deliceText.transform.parent.gameObject.SetActive(false);
                window.title.text = LanguageDictionary.language["PeaceDeal"];
                string description = GainedGold == 0 ? "" : string.Format(LanguageDictionary.language["PeaceDealGoldDescription"], receiver.country.Name, GainedGold);

                foreach (var item in selectedSenderElements) {
                    description += senderElements[item].Name + "\n";
                }
                foreach (var item in selectedReceiverElements) {
                    description += receiverElements[item].Name;
                }
                window.description.text = description;
            }

            sender.country.gold += GainedGold;
            receiver.country.gold -= GainedGold;

            if (!sender.IsMajor) {
                foreach (var item in sender.party.Enemies.countries) {
                    sender.country.relations[item.Key].truceInMonths += Mathf.Abs(UsedWarScore) + 12;
                }
                sender.party.LeaveParty(sender);
            }
            else {
                foreach (var item in receiver.party.Enemies.countries) {
                    receiver.country.relations[item.Key].truceInMonths += Mathf.Abs(UsedWarScore) + 12;
                }
                receiver.party.LeaveParty(receiver);
            }

            foreach (var item in selectedSenderElements) {
                if (senderElements.ContainsKey(item)) {
                    senderElements[item].Execute();
                }
            }
            foreach (var item in selectedReceiverElements) {
                if (receiverElements.ContainsKey(item)) {
                    receiverElements[item].Execute();
                }
            }
        }
    }
}
