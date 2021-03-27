using EuropeanWars.Core.Persons;
using UnityEngine;

namespace EuropeanWars.UI.Windows {
    public class ChoosePersonWindow : MonoBehaviour {
        public delegate void ChoosePerson(Person person);
        private ChoosePerson choosePersonEvent;

        public ChoosePersonWindowElement elementPrefab;
        public Transform content;

        private ChoosePersonWindowElement[] elements;

        public void Initialize(ChoosePerson choosePersonEvent, Person[] persons) {
            this.choosePersonEvent = choosePersonEvent;

            Clear();
            Fill(persons);
        }

        private void Clear() {
            if (elements != null) {
                for (int i = 0; i < elements.Length; i++) {
                    Destroy(elements[i].gameObject);
                }
                elements = null;
            }
        }
        private void Fill(Person[] persons) {
            elements = new ChoosePersonWindowElement[persons.Length];
            for (int i = 0; i < elements.Length; i++) {
                ChoosePersonWindowElement element = Instantiate(elementPrefab, content);
                element.window = this;
                element.SetPerson(persons[i]);
                elements[i] = element;
            }
        }

        public void SelectElement(ChoosePersonWindowElement element) {
            choosePersonEvent.Invoke(element.Person);
            Clear();
            gameObject.SetActive(false);
        }
    }
}
