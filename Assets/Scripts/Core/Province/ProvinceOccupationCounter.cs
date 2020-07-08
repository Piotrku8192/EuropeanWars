using EuropeanWars.Core.Army;

namespace EuropeanWars.Core.Province {
    public class ProvinceOccupationCounter {
        private readonly ProvinceInfo province;

        public ArmyInfo Army { get; private set; }
        public float Progress { get; private set; }

        public ProvinceOccupationCounter(ProvinceInfo province) {
            this.province = province;
        }

        public void SetArmy(ArmyInfo army) {
            Reset();
            Army = army;
        }

        public void Reset() {
            Progress = 0;
        }

        public void UpdateProgress() {
            if (Army != null) {
                if (province.Country.IsInWarAgainstCountry(Army.Country)) {

                }
            }
        }
    }
}
