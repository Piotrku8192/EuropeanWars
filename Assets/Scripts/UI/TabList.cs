using UnityEngine;

namespace EuropeanWars.UI {
    public class TabList : MonoBehaviour {
        public TabElement[] tabs;

        public void Start() {
            foreach (var item in tabs) {
                item.button.onClick.AddListener(() => SelectTab(item));
            }
        }

        public void SelectTab(TabElement tab) {
            UnselectAll();
            tab.Select();
        }

        public void UnselectAll() {
            foreach (var item in tabs) {
                item.Unselect();
            }
        }
    }
}
