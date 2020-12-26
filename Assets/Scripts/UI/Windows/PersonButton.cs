using EuropeanWars.Core.Persons;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace EuropeanWars.UI.Windows {
    public class PersonButton : MonoBehaviour {
        public Person Person { get; private set; }

        public new Text name;
        public Text age;
        public Image portrait;
        public Text description;
        public Text moreInfo;

        public void SetPerson(Person person) {
            if (person == null) {
                throw new ArgumentNullException("person");
            }
            Person = person;
            name.text = Person.name;
            age.text = Person.Age.ToString();
            portrait.sprite = Person.Portrait;
            moreInfo.text = Person.MoreInfo;
        }
    }
}
