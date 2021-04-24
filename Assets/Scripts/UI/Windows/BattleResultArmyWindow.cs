using Assets.Scripts.UI.Windows;
using EuropeanWars.Core.Army;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class BattleResultArmyWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        public CountryButton country;
        public Image armyUnit;
        public Text armySize;
        public Text sizeChange;

        public GameObject unitsObject;
        public BattleResultArmyUnitWindow unitWindowPrefab;
        public List<BattleResultArmyUnitWindow> units = new List<BattleResultArmyUnitWindow>();

        public void OnPointerEnter(PointerEventData eventData) {
            unitsObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData) {
            unitsObject.SetActive(false);
        }

        public void SetArmy(ArmyInfo army, Dictionary<UnitInfo, int> sizeChange) {
            country.SetCountry(army.Country);
            armyUnit.sprite = army.maxUnits.OrderBy(t => t.Value).Last().Key.image;
            armySize.text = $"{army.Size}/{army.MaxSize}";
            armySize.color = Color.Lerp(Color.red, Color.green, (float)army.Size / army.MaxSize);
            this.sizeChange.text = sizeChange.Values.Sum().ToString();

            foreach (var item in sizeChange) {
                BattleResultArmyUnitWindow u = Instantiate(unitWindowPrefab, unitsObject.transform);
                u.SetUnit(item.Key, army.units[item.Key], army.maxUnits[item.Key], item.Value);
                units.Add(u);
            }
        }
    }
}
