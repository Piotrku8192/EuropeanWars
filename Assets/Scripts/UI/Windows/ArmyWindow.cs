using EuropeanWars.Core.Persons;
using UnityEngine;

namespace EuropeanWars.UI.Windows {
    public class ArmyWindow : MonoBehaviour {
        public static ArmyWindow Singleton { get; private set; }
        public GameObject window;

        public RecrutationWindow recrutationWindow;

        public void Awake() {
            Singleton = this;
        }

        public void UpdateWindow() {
            UIManager.Singleton.CloseAllWindows();
            window.SetActive(true);
        }
    }
}
