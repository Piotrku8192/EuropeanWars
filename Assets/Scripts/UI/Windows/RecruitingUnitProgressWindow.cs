using EuropeanWars.Core.Army;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI {
    public class RecruitingUnitProgressWindow : MonoBehaviour {
        public Text unitName;
        public Text size;
        public Text days;
        public Image progressImage;

        public UnitToRecruit unit;

        public void SetUnit(UnitToRecruit unit) {
            this.unit = unit;
            unitName.text = unit.unitInfo.name;
        }

        public void Update() {
            if (unit != null) {
                size.text = (unit.count * unit.unitInfo.recruitSize).ToString();
                days.text = $"{unit.unitInfo.recruitDays - unit.days}/{unit.unitInfo.recruitDays}";
                progressImage.fillAmount = (unit.unitInfo.recruitDays - unit.days) / (float)unit.unitInfo.recruitDays;
                if (unit.days <= 0) {
                    unit = null;
                    Destroy(gameObject);
                }
            }
        }
    }
}
