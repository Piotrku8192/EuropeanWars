using EuropeanWars.Core.Country;
using System;
using UnityEngine;

namespace EuropeanWars.Core.War {
    public abstract class PeaceDealElement {
        public abstract string Name { get; }
        public abstract int WarScoreCost { get; }
        public abstract Color Color { get; }

        public readonly int id;
        public readonly CountryInfo from;
        public readonly CountryInfo to;

        protected PeaceDealElement(PeaceDeal peaceDeal, CountryInfo from, CountryInfo to) {
            this.from = from;
            this.to = to;
            id = peaceDeal.nextElementId;
            peaceDeal.nextElementId++;
        }

        /// <summary>
        /// Returns true if can be used as a element of peaceDeal.
        /// </summary>
        /// <param name="peaceDeal"></param>
        /// <returns></returns>
        public abstract bool CanBeUsed(PeaceDeal peaceDeal);

        public abstract bool CanBeSelected(PeaceDeal peaceDeal);

        public abstract void Execute();

        public virtual bool IsSame(PeaceDealElement element) {
            return Name == element.Name && WarScoreCost == element.WarScoreCost && Color == element.Color
                && from == element.from && to == element.to;
        }
    }
}
