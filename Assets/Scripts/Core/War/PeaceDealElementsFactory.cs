using EuropeanWars.Core.Province;
using System.Collections.Generic;
using System.Linq;

namespace EuropeanWars.Core.War {
    public class PeaceDealElementsFactory {
        private readonly PeaceDeal peaceDeal;

        public PeaceDealElementsFactory(PeaceDeal peaceDeal) {
            this.peaceDeal = peaceDeal;
        }

        public void FillPeaceDeal() {
            if (peaceDeal.sender.IsMajor) {
                foreach (var item in peaceDeal.sender.party.countries) {
                    GetCountryElements(item.Value, peaceDeal.receiver, peaceDeal.senderElements);
                }
            }
            else {
                GetCountryElements(peaceDeal.sender, peaceDeal.receiver, peaceDeal.senderElements);
            }

            if (peaceDeal.receiver.IsMajor) {
                foreach (var item in peaceDeal.receiver.party.countries) {
                    GetCountryElements(item.Value, peaceDeal.sender, peaceDeal.receiverElements);
                }
            }
            else {
                GetCountryElements(peaceDeal.receiver, peaceDeal.sender, peaceDeal.receiverElements);
            }
        }

        private void GetCountryElements(WarCountryInfo country, WarCountryInfo secondCountry, Dictionary<int, PeaceDealElement> target) {
            if (secondCountry.IsMajor) {
                GetProvinceElements(country.enemyOccupatedProvinces, target);
            }
            else {
                GetProvinceElements(country.enemyOccupatedProvinces.Where(t => t.NationalCountry == secondCountry.country).ToList(), target);
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
