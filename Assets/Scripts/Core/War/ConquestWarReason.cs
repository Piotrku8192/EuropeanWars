using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Language;
using EuropeanWars.Core.Province;

namespace EuropeanWars.Core.War {
    public class ConquestWarReason : WarReason {
        public override string Name => string.Format(LanguageDictionary.language["ConquestWarReason"], target.name);
        public override bool ProvinceTakingEnabled => true;

        public readonly ProvinceInfo target;

        public ConquestWarReason(ProvinceInfo target) {
            this.target = target;
        }

        public override bool CanInviteCountryToWar(CountryInfo invitator, CountryInfo friend) {
            return !invitator.IsInWarAgainstCountry(friend) && invitator.relations[friend].relations[(int)DiplomaticRelation.Alliance]; //TODO: Add || invitator.vassals.Contains(friend) when vassals are implemented
        }
    }
}
