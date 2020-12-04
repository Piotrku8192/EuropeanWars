using EuropeanWars.Core.Army;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Time;
using EuropeanWars.Core.War;
using EuropeanWars.Network;
using Lidgren.Network;

namespace EuropeanWars.Core.AI {
    public abstract class CountryAI {
        protected readonly CountryInfo country;
        private int warReason;

        protected CountryAI(CountryInfo country) {
            this.country = country;
            TimeManager.lateOnDayElapsed += OnDayElapsed;
            TimeManager.lateOnDayElapsed += CheckIfCountryIsPlayer;
            TimeManager.lateOnMonthElapsed += OnMonthElapsed;
            TimeManager.lateOnYearElapsed += OnYearElapsed;
        }

        public void Delete() {
            TimeManager.lateOnDayElapsed -= OnDayElapsed;
            TimeManager.lateOnDayElapsed -= CheckIfCountryIsPlayer;
            TimeManager.lateOnMonthElapsed -= OnMonthElapsed;
            TimeManager.lateOnYearElapsed -= OnYearElapsed;
        }

        private void CheckIfCountryIsPlayer() {
            if (country.isPlayer) {
                GameInfo.countryAIs.Remove(country);
                Delete();
            }
        }

        protected abstract void OnDayElapsed();
        protected abstract void OnMonthElapsed();
        protected abstract void OnYearElapsed();

        public virtual bool IsDiplomaticRelationChangeAccepted(DiplomaticRelation relation, CountryInfo sender) {
            CountryRelation r = country.relations[sender];
            switch (relation) {
                case DiplomaticRelation.Alliance:
                    return r.Points > 60;
                case DiplomaticRelation.MilitaryAccess:
                    return r.Points > 30;
                case DiplomaticRelation.TradeAgreament:
                    return r.Points > 40;
                case DiplomaticRelation.RoyalMariage:
                    return r.Points > 0;
                default:
                    return false;
            }
        }

        protected virtual void BuildBuildingInSlot(BuildingInfo building, ProvinceInfo province, int slot) {
            province.BuildBuilding(building, slot);
        }
        protected virtual void RecruitArmy(UnitInfo unit, ProvinceInfo province, int count) {
            country.EnqueueUnitToRecruit(unit, province, count);
        }
        protected virtual void FabricateClaim(ProvinceInfo province) {
            province.FabricateClaim(country);
        }
        protected virtual void DeclareWar(CountryInfo _country, int reason) {
            if (country.relations[_country].truceInMonths == 0) {
                WarReasonFactory factory = new WarReasonFactory(country, _country);
                WarReason w = factory.GetReasons()[reason];
                DiplomacyManager.DeclareWar(w, country, _country);
            }
        }
    }
}
