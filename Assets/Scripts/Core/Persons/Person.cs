using EuropeanWars.Core.Country;
using EuropeanWars.Core.Time;
using System;
using UnityEngine;

namespace EuropeanWars.Core.Persons {
    public class Person {
        public readonly int id;
        public readonly string name;
        public readonly int birthYear;
        public readonly int deathYear;

        public CountryInfo country;

        public Sprite Portrait { get; private set; } //TODO: Maybe change this to dynamic portrait as person is older
        public int Age { get; private set; }
        public virtual string Speciality => "-";
        public virtual string MoreInfo => "-";

        public Person(string name, int birthYear, int deathYear, CountryInfo country, Sprite portrait) { //TODO: In the future refactorize it to be able to load it from gameData
            id = GameInfo.persons.Count;
            GameInfo.persons.Add(id, this);

            this.name = name;
            this.birthYear = birthYear;
            this.deathYear = deathYear;
            this.country = country;
            this.Portrait = portrait;
            Age = TimeManager.year - birthYear;

            TimeManager.onYearElapsed += CalculateAge;
        }

        private void CalculateAge() {
            Age++;
            if (birthYear + Age == deathYear) {
                OnDeath();
            }
        }

        protected virtual void OnDeath() {
            throw new NotImplementedException();
        }
    }
}
