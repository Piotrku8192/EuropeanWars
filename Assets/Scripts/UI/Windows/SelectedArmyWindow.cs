using EuropeanWars.Core.Army;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class SelectedArmyWindow : MonoBehaviour
    {
        public static SelectedArmyWindow Singleton { get; private set; }

        public GameObject windowObject;

        public ArmyButton armyButtonPrefab;
        public Transform armiesContent;

        public ArmyUnitButton armyUnitButtonPrefab;

        public ArmyInfo SelectedArmy { get; private set; }
        public Transform unitsContent;
        public Text armySize;

        public ArmyInfo MovingArmy { get; private set; }
        public GameObject movingUnitsObject;
        public Transform movingUnitsContent;

        private Dictionary<ArmyInfo, ArmyButton> armies = new Dictionary<ArmyInfo, ArmyButton>();
        private List<ArmyUnitButton> units = new List<ArmyUnitButton>();
        private List<ArmyUnitButton> movingUnits = new List<ArmyUnitButton>();

        public void Awake() {
            Singleton = this;
        }

        public void Update() {
            if (SelectedArmy != null) {
                armySize.text = $"{SelectedArmy.Size}/{SelectedArmy.MaxSize}";
                armySize.color = Color.Lerp(Color.red, Color.green, SelectedArmy.Size / (float)SelectedArmy.MaxSize);

                if (MovingArmy != null && SelectedArmy.Province != MovingArmy.Province) {
                    movingUnitsObject.SetActive(false);
                    MovingArmy = null;
                }
            }
        }

        public void UpdateWindow() {
            ArmyInfo a = SelectedArmy;
            SelectedArmy = null;
            SelectArmy(a);
            ArmyInfo m = MovingArmy;
            MovingArmy = null;
            SelectMovingArmy(m);
        }

        public void OnClose() {
            ArmyInfo.UnselectAll();
        }

        public void AddArmy(ArmyInfo army) {
            if (windowObject.activeInHierarchy == false) {
                UIManager.Singleton.CloseAllWindows(false);
                windowObject.SetActive(true);
            }
            ArmyButton a = Instantiate(armyButtonPrefab, armiesContent);
            a.SetArmy(army);
            armies.Add(army, a);
            SelectArmy(army);
        }

        public void RemoveArmy(ArmyInfo army) {
            Destroy(armies[army].gameObject);
            armies.Remove(army);

            if (armies.Count == 0) {
                movingUnitsObject.SetActive(false);
                windowObject.SetActive(false);
                SelectedArmy = null;
                return;
            }

            if (SelectedArmy == army) {
                SelectArmy(armies.First().Key);
            }
        }

        public void SelectArmy(ArmyInfo army) {
            if (SelectedArmy == army) {
                return;
            }
            else if (MovingArmy == army || MovingArmy?.Province != army.Province) {
                movingUnitsObject.SetActive(false);
                MovingArmy = null;
            }

            foreach (var item in armies) {
                item.Value.isSelected = false;
            }
            armies[army].isSelected = true;
            SelectedArmy = army;

            foreach (var item in units) {
                Destroy(item.gameObject);
            }
            units.Clear();
            foreach (var item in army.units) {
                ArmyUnitButton b = Instantiate(armyUnitButtonPrefab, unitsContent);
                b.SetUnit(item.Key, army);
                units.Add(b);
            }
        }

        public void SelectMovingArmy(ArmyInfo army) {
            if (SelectedArmy == null || SelectedArmy == army || MovingArmy == army || SelectedArmy.Province != army.Province) {
                return;
            }

            foreach (var item in movingUnits) {
                Destroy(item.gameObject);
            }
            movingUnits.Clear();
            movingUnitsObject.SetActive(true);
            MovingArmy = army;
            foreach (var item in army.units) {
                ArmyUnitButton b = Instantiate(armyUnitButtonPrefab, movingUnitsContent);
                b.SetUnit(item.Key, army);
                movingUnits.Add(b);
            }
        }
    }
}
