using EuropeanWars.Core.Province;
using System.Collections.Generic;

namespace EuropeanWars.Core.War {
    public class PeaceDealElementsFactory {
        private readonly PeaceDeal peaceDeal;

        public PeaceDealElementsFactory(PeaceDeal peaceDeal) {
            this.peaceDeal = peaceDeal;
        }

        public void FillPeaceDeal() {
            if (peaceDeal.sender.IsMajor) {
                foreach (var item in peaceDeal.sender.party.countries) {
                    peaceDeal.senderElements.AddRange(GetProvinceElements(item.Value.enemyOccupatedProvinces));
                }
            }
            else {
                peaceDeal.senderElements.AddRange(GetProvinceElements(peaceDeal.sender.enemyOccupatedProvinces));
            }

            if (peaceDeal.receiver.IsMajor) {
                foreach (var item in peaceDeal.receiver.party.countries) {
                    peaceDeal.receiverElements.AddRange(GetProvinceElements(item.Value.enemyOccupatedProvinces));
                }
            }
            else {
                peaceDeal.receiverElements.AddRange(GetProvinceElements(peaceDeal.sender.enemyOccupatedProvinces));
            }
        }

        private PeaceDealElement[] GetProvinceElements(List<ProvinceInfo> provinces) {
            List<PeaceDealElement> result = new List<PeaceDealElement>();
            foreach (var item in provinces) {
                ProvincePeaceDealElement element = new ProvincePeaceDealElement(item);
                if (element.CanBeUsed(peaceDeal)) {
                    result.Add(element);
                }
            }

            return result.ToArray();
        }
    }
}
