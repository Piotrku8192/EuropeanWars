using EuropeanWars.Core.Country;
using EuropeanWars.Core.Province;

namespace EuropeanWars.Core.War {
    public class ConquestWarReason : WarReason {
        public override string Name => "Roszczenia do prowincji " + target.name; //TODO: Add translate
        public override bool ProvinceTakingEnabled => true;

        public readonly ProvinceInfo target;

        public ConquestWarReason(ProvinceInfo target) {
            this.target = target;
        }

        public override bool CanInviteCountryToWar(CountryInfo invitator, CountryInfo friend) {
            return !invitator.IsInWarAgainstCountry(friend) && invitator.alliances.ContainsKey(friend);//TODO: Add || invitator.vassals.Contains(friend) when vassals are implemented
        }
    }
}
