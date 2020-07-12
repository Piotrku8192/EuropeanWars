using EuropeanWars.Core.Army;
using System.Collections.Generic;
using UnityEngine;

namespace EuropeanWars.UI.Windows {
    public class SelectedArmyWindow : MonoBehaviour
    {
        public static SelectedArmyWindow Singleton { get; private set; }

        public GameObject windowObject;

        public ArmyButton armyButtonPrefab;
        public Transform armiesContent;
        private Dictionary<ArmyInfo, ArmyButton> armies = new Dictionary<ArmyInfo, ArmyButton>();

        public void Awake() {
            Singleton = this;
        }

        public void AddArmy(ArmyInfo army) {
            if (windowObject.activeInHierarchy == false) {
                UIManager.Singleton.CloseAllWindows(false);
                windowObject.SetActive(true);
            }
            ArmyButton a = Instantiate(armyButtonPrefab, armiesContent);
            a.SetArmy(army);
            armies.Add(army, a);
        }

        public void RemoveArmy(ArmyInfo army) {
            Destroy(armies[army].gameObject);
            armies.Remove(army);

            if (armies.Count == 0) {
                windowObject.SetActive(false);
            }
        }
    }
}
