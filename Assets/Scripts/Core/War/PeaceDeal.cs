using System.Collections.Generic;

namespace EuropeanWars.Core.War {
    public class PeaceDeal {
        public readonly WarInfo war;
        public readonly WarCountryInfo sender;
        public readonly WarCountryInfo receiver;

        public Dictionary<int, PeaceDealElement> senderElements = new Dictionary<int, PeaceDealElement>();
        public Dictionary<int, PeaceDealElement> receiverElements = new Dictionary<int, PeaceDealElement>();

        public List<int> selectedSenderElements = new List<int>();
        public List<int> selectedReceiverElements = new List<int>();

        public int GainedGold { get; private set; }
        public int UsedWarScore { get; private set; }

        public PeaceDeal(WarInfo war, WarCountryInfo sender, WarCountryInfo receiver) {
            this.war = war;
            this.sender = sender;
            this.receiver = receiver;
            PeaceDealElementsFactory factory = new PeaceDealElementsFactory(this);
            factory.FillPeaceDeal();
        }

        public void SelectSenderElement(PeaceDealElement element) {
            if (!selectedSenderElements.Contains(element.id)) {

            }
        }

        public void UnselectSenderElement(PeaceDealElement element) {

        }

        public void SelectReceiverElement(PeaceDealElement element) {

        }

        public void UnselectReceiverElement(PeaceDealElement element) {

        }
    }
}
