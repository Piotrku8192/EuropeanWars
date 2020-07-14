using EuropeanWars.Core.Province;

namespace EuropeanWars.Core.War {
    public class ProvincePeaceDealElement : PeaceDealElement {
        public override string Name => $"{province.NationalCountry.name} oddaje prowincję {province.name} dla {province.Country.name}"; //TODO: Make this translating when translate system is done.

        public readonly ProvinceInfo province;
        public readonly float costModifier;

        public ProvincePeaceDealElement(ProvinceInfo province) : base(province.NationalCountry, province.Country) {
            this.province = province;
            costModifier = 1.0f;
        }

        public override bool CanBeUsed(PeaceDeal peaceDeal) {
            return peaceDeal.war.warReason.ProvinceTakingEnabled;
        }
    }
}
