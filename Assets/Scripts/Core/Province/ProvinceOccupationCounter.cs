using EuropeanWars.Core.Army;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.War;
using EuropeanWars.UI.Windows;
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
                if (ProvinceWindow.Singleton.province == province) {
                    ProvinceWindow.Singleton.UpdateWindow(province);
                }
            }
        }

        public void UpdateProgress() {
            if (Army != null && Army.Province == province) {
                if (province.Country.IsInWarAgainstCountry(Army.Country)) {
                    int artilleries = Army.units.Where(t => t.Key.type == UnitType.Artillery).Sum(t => t.Value);
                    Progress += (float)1 / province.defense + Mathf.Clamp(artilleries, 0, 50) * 0.1f; //TODO: Add these values to GameStatistics

                    if (daysToNext <= 0) {
                        daysToNext = daysBetweenAttacks;
                        attackCounter.CountAttack();
                    }
                    daysToNext--;

                    if (Progress >= 100) {
                        OnDefendersEmpty();
                    }

                    return;
                }
            }
            FindNewOccupant();
        }

        private void FindNewOccupant() {
            SetArmy(province.armies.Where(t => province.Country.IsInWarAgainstCountry(t.Country)).FirstOrDefault());
        }

        private void OnAttackersEmpty() {
            Army.Delete();
            FindNewOccupant();
        }

        private void OnDefendersEmpty() {
            CountryInfo c = province.Country;
            WarInfo armyWar = Army.Country.GetWarAgainstCountry(province.NationalCountry);
            WarInfo occupantWar = c.GetWarAgainstCountry(province.NationalCountry);

            if (c == province.NationalCountry && Army.Country != province.NationalCountry) {
                if (armyWar != null) {
                    province.SetCountry(Army.Country);
                    Army.Country.wars[armyWar].AddEnemyOccupatedProvince(province);
                }
            }
            else if (Army.Country == province.NationalCountry && c != province.NationalCountry) {
                if (occupantWar != null) {
                    province.SetCountry(Army.Country);
                    c.wars[occupantWar].RemoveEnemyOccupatedProvince(province);
                }
            }
            else if (Army.Country != province.NationalCountry && c != province.NationalCountry && Army.Country.IsInWarAgainstCountry(c)) {
                if (occupantWar != null) {
                    province.SetCountry(province.NationalCountry);
                    c.wars[occupantWar].RemoveEnemyOccupatedProvince(province);
                }
            }
            province.RefreshFogOfWarInRegion();
            //TODO: Add uprising statement

            FindNewOccupant();
        }
    }
}
