using System;

namespace EuropeanWars.Core.War {
    public class DynamicPeaceDeal : PeaceDeal {
        public DynamicPeaceDeal(WarInfo war, WarCountryInfo sender, WarCountryInfo receiver) : base(war, sender, receiver) {
        }

        public PeaceDeal GetFinalPeaceDeal() {
            throw new NotImplementedException();
        }
    }
}
