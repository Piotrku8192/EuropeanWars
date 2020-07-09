using EuropeanWars.Core.Army;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.Core.Province {
    public class ProvinceOccupationCounter {
        private readonly ProvinceInfo province;
        private readonly int daysBetweenAttacks;

        public ArmyInfo Army { get; private set; }
        public float Progress { get; private set; }

        private int daysToNext;
        private ArmyAttackCounter attackCounter;

        public ProvinceOccupationCounter(ProvinceInfo province, int daysBetweenAttacks = 4) {
            this.province = province;
            this.daysBetweenAttacks = daysBetweenAttacks;
        }

        public void Reset() {
            Army = null;
            Progress = 0;
            daysToNext = daysBetweenAttacks;
            attackCounter = null;
        }

        private void SetArmy(ArmyInfo army) {
            Reset();
            if (army != null) {
                Army = army;
                attackCounter = new ArmyAttackCounter(Army.units, province.garnison, GameStatistics.occupantArmyAttackModifier,
                    GameStatistics.occupatedArmyAttackModifier, OnAttackersEmpty, OnDefendersEmpty); 
            }
        }

        public void UpdateProgress() {
            if (Army != null) {
                if (province.Country.IsInWarAgainstCountry(Army.Country)) {
                    int artilleries = Army.units.Where(t => t.Key.type == UnitType.Artillery).Sum(t => t.Value);
                    Progress += (float)1 / province.defense + Mathf.Clamp(artilleries, 0, 50) * 0.1f; //TODO: Add these values to GameStatistics
                    
                    if (daysToNext <= 0) {
                        daysToNext = daysBetweenAttacks;
                        attackCounter.CountAttack();
                    }
                    daysToNext--;
                    return;
                }
            }
            FindNewOccupant();
        }

        private void FindNewOccupant() {
            SetArmy(province.armies.SingleOrDefault(t => province.Country.IsInWarAgainstCountry(t.Country)));
        }

        private void OnAttackersEmpty() {
            Army.Delete();
            FindNewOccupant();
        }

        private void OnDefendersEmpty() {
            province.SetCountry(Army.Country);
            FindNewOccupant();
        }
    }
}
