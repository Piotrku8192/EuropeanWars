using EuropeanWars.Core.War;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class PeaceDealElementButton : MonoBehaviour {
        public PeaceDealElement element;
        public bool isSenderElement;

        public Text description;
        public Text cost;

        public Image selectionMask;

        public bool IsSelected { get; private set; }

        public void SetElement(PeaceDealElement element, bool isSenderElement) {
            this.element = element;
            this.isSenderElement = isSenderElement;
            description.text = element.Name;
            cost.text = element.WarScoreCost + "%";
            cost.color = element.Color;
        }

        public void OnClick() {
            if (PeaceDealWindow.Singleton.peaceDeal != null) {
                IsSelected = !IsSelected;
                selectionMask.gameObject.SetActive(IsSelected);
                if (IsSelected) {
                    if (isSenderElement) {
                        PeaceDealWindow.Singleton.peaceDeal.SelectSenderElement(element);
                    }
                    else {
                        PeaceDealWindow.Singleton.peaceDeal.SelectReceiverElement(element);
                    }
                }
                else {
                    if (isSenderElement) {
                        PeaceDealWindow.Singleton.peaceDeal.UnselectSenderElement(element);
                    }
                    else {
                        PeaceDealWindow.Singleton.peaceDeal.UnselectReceiverElement(element);
                    }
                }
            }
        }
    }
}
