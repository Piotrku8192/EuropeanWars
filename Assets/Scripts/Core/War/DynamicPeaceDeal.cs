using System.Linq;

namespace EuropeanWars.Core.War {
    public class DynamicPeaceDeal : PeaceDeal {
        public DynamicPeaceDeal(WarInfo war, WarCountryInfo sender, WarCountryInfo receiver) : base(war, sender, receiver) {
        }

        public PeaceDeal GetFinalPeaceDeal() {
            PeaceDeal result = new PeaceDeal(war, sender, receiver);
            foreach (var item in selectedSenderElements) {
                result.SelectSenderElement(result.senderElements.Where(t =>
                t.Value.IsSame(senderElements[item])).FirstOrDefault().Value);
            }
            foreach (var item in selectedReceiverElements) {
                result.SelectReceiverElement(result.receiverElements.Where(t =>
                t.Value.IsSame(receiverElements[item])).FirstOrDefault().Value);
            }

            result.ChangeGold(GainedGold / 10);
            return result;
        }
    }
}
