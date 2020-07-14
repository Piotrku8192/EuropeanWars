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
                    GetProvinceElements(item.Value.enemyOccupatedProvinces, peaceDeal.senderElements);
                }
            }
            else {
                GetProvinceElements(peaceDeal.sender.enemyOccupatedProvinces, peaceDeal.senderElements);
            }

            if (peaceDeal.receiver.IsMajor) {
                foreach (var item in peaceDeal.receiver.party.countries) {
                    GetProvinceElements(item.Value.enemyOccupatedProvinces, peaceDeal.receiverElements);
                }
            }
            else {
                GetProvinceElements(peaceDeal.sender.enemyOccupatedProvinces, peaceDeal.receiverElements);
            }
        }

        private void GetProvinceElements(List<ProvinceInfo> provinces, Dictionary<int, PeaceDealElement> target) {
            foreach (var item in provinces) {
                ProvincePeaceDealElement element = new ProvincePeaceDealElement(item, peaceDeal);
                if (element.CanBeUsed(peaceDeal)) {
                    target.Add(element.id, element);
                }
            }
        }
    }
}
