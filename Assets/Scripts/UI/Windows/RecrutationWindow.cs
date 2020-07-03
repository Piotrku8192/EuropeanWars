using Boo.Lang;
using EuropeanWars.Core;
using EuropeanWars.Core.Army;
using EuropeanWars.Core.Pathfinding;
using EuropeanWars.GameMap;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI {
    public class RecrutationWindow : MonoBehaviour {
        public List<UnitButton> units = new List<UnitButton>();
        public Transform unitsContent;
        public UnitButton unitButtonPrefab;

        [Header("UnitInfo")]
        public UnitInfo selectedUnit;
        public Text unitName;
        public Image unitImage;

        [Header("RecruitingUnits")]
        public Transform recruitingUnitsListContent;
        public RecruitingUnitProgressWindow recruitingUnitProgressWindowPrefab;

        public void OnDisable() {
            if (MapPainter.mapMode == MapMode.Recrutation) {
                MapPainter.PaintMap(MapMode.Countries);
            }
        }

        public void Start() {
            foreach (var item in GameInfo.PlayerCountry.units) {
                UnitButton b = Instantiate(unitButtonPrefab, unitsContent);
                b.SetUnit(item);
                units.Add(b);
            }
            units.FirstOrDefault().OnClick();
        }

        public void SelectUnit(UnitInfo unit) {
            if (unit != null) {
                selectedUnit = unit;
                unitName.text = unit.name;
                unitImage.sprite = unit.image;
            }
        }

        public void ShowRecrutationMap() {
            if (selectedUnit != null) {
                MapPainter.PaintMap(MapMode.Recrutation);
            }
        }

        public void AddRecruitingUnit(UnitToRecruit unit) {
            RecruitingUnitProgressWindow window = Instantiate(recruitingUnitProgressWindowPrefab, recruitingUnitsListContent);
            window.SetUnit(unit);
        }
    }
}

