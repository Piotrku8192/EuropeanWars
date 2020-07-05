using Boo.Lang;

namespace EuropeanWars.Core.War {
    public class WarParty {
        public readonly WarInfo war;
        public readonly WarParty enemies;
        public readonly WarCountryInfo major;
        public readonly List<WarCountryInfo> countries = new List<WarCountryInfo>();
        public WarScore WarScore { get; private set; }
    }
}
