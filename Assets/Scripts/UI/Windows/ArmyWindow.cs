using Boo.Lang;
using EuropeanWars.Core.Army;
using System;
using UnityEngine;

namespace EuropeanWars.UI {
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
