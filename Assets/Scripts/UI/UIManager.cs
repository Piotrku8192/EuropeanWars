using EuropeanWars.Core;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI {
    public class UIManager : MonoBehaviour {
        public static UIManager Singleton { get; private set; }

        public GameObject lobby;
        public GameObject ui;
        public Image playerCountryCrest;

        public GameObject[] windows;

        public void Awake() {
            Singleton = this;
        }

        public void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                CloseAllWindows();
            }
        }

        public void CloseAllWindows() {
            foreach (var item in windows) {
                item.SetActive(false);
            }
        }
    }
}
