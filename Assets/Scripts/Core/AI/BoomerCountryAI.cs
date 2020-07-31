using EuropeanWars.Core.Country;
using System;

namespace EuropeanWars.Core.AI {
    public class BoomerCountryAI : CountryAI {
        public BoomerCountryAI(CountryInfo country) : base(country) {
        }

        protected override void OnDayElapsed() {
            throw new NotImplementedException();
        }

        protected override void OnMonthElapsed() {
            throw new NotImplementedException();
        }

        protected override void OnYearElapsed() {
            throw new NotImplementedException();
        }
    }
}
