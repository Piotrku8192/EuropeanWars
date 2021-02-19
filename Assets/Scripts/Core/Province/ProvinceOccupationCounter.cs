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

        private bool areAttackersEmpty;
        private bool areDefendersEmpty;

        public ProvinceOccupationCounter(ProvinceInfo province, int daysBetweenAttacks = 4) {
            this.province = province;
            this.daysBetweenAttacks = daysBetweenAttacks;
        }

        public void Reset() {
            Army = null;
            Progress = 0;
            daysToNext = daysBetweenAttacks;
            attackCounter = null;
            areAttackersEmpty = false;
            areDefendersEmpty = false;
        }

        private void SetArmy(ArmyInfo army) {
            Reset();
            if (army != null) {
                Army = army;
                float[] attackersModifier = new float[GameStatistics.occupantArmyAttackModifier.Length];
                for (int i = 0; i < attackersModifier.Length; i++) {
                    attackersModifier[i] = GameStatistics.occupantArmyAttackModifier[i] + army.Country.armyAttackModifier;
                }
                float[] defendersModifier = new float[GameStatistics.occupatedArmyAttackModifier.Length];
                for (int i = 0; i < defendersModifier.Length; i++) {
                    defendersModifier[i] = GameStatistics.occupatedArmyAttackModifier[i] + army.Country.armyAttackModifier;
                }

                attackCounter = new ArmyAttackCounter(Army.units, province.garnison, attackersModifier,
                    defendersModifier, () => areAttackersEmpty = true, () => areDefendersEmpty = true);
                if (ProvinceWindow.Singleton.province == province) {
                    ProvinceWindow.Singleton.UpdateWindow(province);
                }
            }
        }

        public void UpdateProgress() {
            if (Army != null && province.armies.Contains(Army)) {
                if (province.Country.IsInWarAgainstCountry(Army.Country)) { //TODO: Add uprising here
                    Progress += ((float)1 / province.defense) + (Mathf.Clamp(Army.Artilleries, 0, GameStatistics.maxArtilleryOccupationBonus)
                        * GameStatistics.artilleryOccupationBonusModifier);

                    if (daysToNext <= 0) {
                        daysToNext = daysBetweenAttacks;
                        int killedAttackers = 0;
                        int killedDefenders = 0;

                        WarInfo war = province.Country.GetWarAgainstCountry(Army.Country);
                        ArmyInfo army = Army;
                        attackCounter.CountAttack(out killedDefenders, out killedAttackers);

                        province.Country.wars[war].killedEnemies += killedAttackers;
                        province.Country.wars[war].killedLocal += killedDefenders;

                        army.Country.wars[war].killedEnemies += killedDefenders;
                        army.Country.wars[war].killedLocal += killedAttackers;
                    }
                    daysToNext--;

                    if (Progress >= 100 || areDefendersEmpty) {
                        OnDefendersEmpty();
                        Reset();
                    }
                    else if (areAttackersEmpty) {
                        OnAttackersEmpty();
                        Reset();
                    }

                    return;
                }
            }
            else {
                FindNewOccupant();
            }
        }

        public void FindNewOccupant() {
            SetArmy(province.armies.FirstOrDefault(t => province.Country.IsInWarAgainstCountry(t.Country) && !t.route.Any(x => x != province)));
        }

        private void OnAttackersEmpty() {
            Army.DeleteLocal();
            FindNewOccupant();
        }

        private void OnDefendersEmpty() {
            if (Army != null) {
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
            }

            FindNewOccupant();
        }
    }
}
