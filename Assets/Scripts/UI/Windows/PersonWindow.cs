using EuropeanWars.Core.Persons;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class PersonWindow : MonoBehaviour {
        public Person Person { get; private set; }

        public new Text name;
        public Text age;
        public Image portrait;
        public Text description;
        public Text speciality;
        public Text moreInfo;

        public void SetPerson(Person person) {
            Person = person ?? throw new ArgumentNullException("person");
            name.text = Person.firstName;
            age.text = Person.Age.ToString();
            portrait.sprite = Person.Portrait;
            speciality.text = Person.Speciality;
            moreInfo.text = Person.MoreInfo;
        }
    }
}
