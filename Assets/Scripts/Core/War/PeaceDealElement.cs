using EuropeanWars.Core.Country;
using System;

namespace EuropeanWars.Core.War {
    public abstract class PeaceDealElement {
        private static int nextId;

        public abstract string Name { get; }

        public readonly int id;
        public readonly CountryInfo from;
        public readonly CountryInfo to;

        protected PeaceDealElement(CountryInfo from, CountryInfo to) {
            this.from = from;
            this.to = to;
            id = nextId;
            nextId++;
        }

        /// <summary>
        /// Returns true if can be used as a element of peaceDeal.
        /// </summary>
        /// <param name="peaceDeal"></param>
        /// <returns></returns>
        public abstract bool CanBeUsed(PeaceDeal peaceDeal);
    }
}
