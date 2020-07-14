using EuropeanWars.Core.Country;

namespace EuropeanWars.Core.War {
    public class GoldPeaceDealElement : PeaceDealElement {
        public override string Name => throw new System.NotImplementedException();

        public readonly int gold;

        public GoldPeaceDealElement(CountryInfo from, CountryInfo to, int gold) : base(from, to) {
            this.gold = gold;
        }

        public override bool CanBeUsed(PeaceDeal peaceDeal) {
            return true;
        }
    }
}
