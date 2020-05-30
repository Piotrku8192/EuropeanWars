using EuropeanWars.Core.Country;
using EuropeanWars.Core.Time;
using System.Collections.Generic;

namespace EuropeanWars.Core.Diplomacy {
    public class DiplomaticRelation {
        public List<CountryInfo> countries = new List<CountryInfo>();
        public int days;

        public DiplomaticRelation() {
            TimeManager.onDayElapsed += OnDayElapsed;
        }

        protected virtual void OnDayElapsed() {
            days++;
        }
    }
}
