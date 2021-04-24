using EuropeanWars.Core.Persons;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace EuropeanWars.UI.Windows {
    public class PersonButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        public Person Person { get; private set; }
        public PersonWindow windowPrefab;
        public DescriptionText moreInfo;
        public Image portrait;
        public Sprite nullSprite;

        public Button showMoreInfoButton;

        public virtual void SetPerson(Person person) {
            Person = person;

            if (person == null) {
                portrait.sprite = nullSprite;
                moreInfo.text = "";
            }
            else {
                portrait.sprite = person.Portrait;
                moreInfo.text = person.MoreInfo;
            }
        }

        public void ShowMoreInfo() {
            PersonWindow w = Instantiate(windowPrefab, UIManager.Singleton.ui.transform);
            w.SetPerson(Person);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            showMoreInfoButton.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData) {
            showMoreInfoButton.gameObject.SetActive(false);
        }
    }
}
