using EuropeanWars.Core.Country;
using EuropeanWars.Core.Time;

namespace EuropeanWars.Core.AI {
    public abstract class CountryAI {
        protected readonly CountryInfo country;

        protected CountryAI(CountryInfo country) {
            this.country = country;
            TimeManager.onDayElapsed += OnDayElapsed;
            TimeManager.onDayElapsed += CheckIfCountryIsPlayer;
            TimeManager.onYearElapsed += OnMonthElapsed;
            TimeManager.onYearElapsed += OnYearElapsed;
        }

        ~CountryAI() {
            TimeManager.onDayElapsed -= OnDayElapsed;
            TimeManager.onDayElapsed -= CheckIfCountryIsPlayer;
            TimeManager.onYearElapsed -= OnMonthElapsed;
            TimeManager.onYearElapsed -= OnYearElapsed;
        }

        private void CheckIfCountryIsPlayer() {
            if (country.isPlayer) {
                GameInfo.countryAIs.Remove(country);
            }
        }

        protected abstract void OnDayElapsed();
        protected abstract void OnMonthElapsed();
        protected abstract void OnYearElapsed(); 
    }
}
