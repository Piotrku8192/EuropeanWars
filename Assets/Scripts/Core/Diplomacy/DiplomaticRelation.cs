using EuropeanWars.Core.Country;
using EuropeanWars.Core.Time;
using System.Collections.Generic;

namespace EuropeanWars.Core.Diplomacy {
    public class DiplomaticRelation {
        public List<CountryInfo> countries = new List<CountryInfo>();
        public string name;
        public int days;

        public DiplomaticRelation() {
            TimeManager.onDayElapsed += OnDayElapsed;
            OnStart();
        }

        ~DiplomaticRelation() {
            OnEnd();
        }

        protected virtual void OnDayElapsed() {
            days++;
        }

        protected virtual void OnStart() {

        }

        protected virtual void OnEnd() {

        }
    }
}
