using EuropeanWars.Network;
using Lidgren.Network;
using System.Collections.Generic;

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

        public int SenderWarScore => sender.IsMajor ? sender.party.PercentWarScore : sender.PercentWarScore;
        public int ReceiverWarScore => receiver.IsMajor ? receiver.party.PercentWarScore : receiver.PercentWarScore;

        public PeaceDeal(WarInfo war, WarCountryInfo sender, WarCountryInfo receiver) {
            this.war = war;
            this.sender = sender;
            this.receiver = receiver;
            PeaceDealElementsFactory factory = new PeaceDealElementsFactory(this);
            factory.FillPeaceDeal();
        }

        /// <summary>
        /// Adds gainedGold by change * 10. Every 1 change costs 1% warScore.
        /// </summary>
        /// <param name="change">GainedGold change in 10 size bits.</param>
        public void ChangeGold(int change) {
            GainedGold += change * 10;
            UsedWarScore += change;
        }

        public void SelectSenderElement(PeaceDealElement element) {
            if (!selectedSenderElements.Contains(element.id)) {
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
            if (!selectedReceiverElements.Contains(element.id)) {
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

        }

        public void Execute() {

        }
    }
}
