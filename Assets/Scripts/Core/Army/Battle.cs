﻿using EuropeanWars.Core.Province;
using EuropeanWars.Core.War;
using EuropeanWars.UI.Windows;
using System;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.Core.Army {
    public class Battle {
        private readonly BattleArmyGroup attackers;
        private readonly BattleArmyGroup defenders;
        private readonly ProvinceInfo province;
        private readonly ArmyAttackCounter attackCounter;

        private int killedAttackers;
        private int killedDefenders;

        private bool ended;

        public Battle(BattleArmyGroup attacker, BattleArmyGroup defender, ProvinceInfo province) {
            this.attackers = attacker;
            this.defenders = defender;
            this.province = province;
            attackCounter = new ArmyAttackCounter(attacker.GetUnits(), defender.GetUnits(), GameStatistics.battleAttackerArmyAttackModifier,
                GameStatistics.battleDefenderArmyAttackModifier, () => ended = true, () => ended = true);

            PlayBattle();
        }

        private void PlayBattle() {
            int attacksCount = Mathf.CeilToInt((attackers.Size + defenders.Size) * GameStatistics.battleAttacksCountModifier);
            for (int i = 0; i < attacksCount; i++) {
                if (ended) {
                    break;
                }

                attackCounter.CountAttack(out int kd, out int ka);
                killedDefenders += kd;
                killedAttackers += ka;
            }
            attackers.UpdateArmies();
            defenders.UpdateArmies();

            foreach (var item in defenders.Armies) {
                if (item.Size <= 0) {
                    item.DeleteLocal();
                } 
            }
            foreach (var item in attackers.Armies) {
                if (item.Size <= 0) {
                    item.DeleteLocal();
                }
            }
            if (defenders.Size <= 0) {
                OnAttackersWin();
            }
            else if (attackers.Size <= 0) {
                OnDefendersWin();
            }
            else if (killedDefenders >= killedAttackers) {
                OnAttackersWin();
                foreach (var item in defenders.Armies) {
                    ProvinceInfo[] ps = item.Country.provinces.Where(t => t != province).ToArray();
                    if (ps.Length == 0) {
                        ps = new ProvinceInfo[1] { province.neighbours.First(t => t.isLand && t.isInteractive) };
                    }
                    item.GenerateRoute(ps[GameInfo.random.Next(0, ps.Count() - 1)]);
                    item.isMoveLocked = true;
                }
            }
            else if (killedAttackers > killedDefenders) {
                OnDefendersWin();
                foreach (var item in attackers.Armies) {
                    ProvinceInfo[] ps = item.Country.provinces.Where(t => t != province).ToArray();
                    if (ps.Length == 0) {
                        ps = new ProvinceInfo[1] { province.neighbours.First(t => t.isLand && t.isInteractive) };
                    }
                    item.GenerateRoute(ps[GameInfo.random.Next(0, ps.Count() - 1)]);
                    item.isMoveLocked = true;
                }
            }
        }

        private void OnAttackersWin() {
            if (attackers.Armies.Any(t => t.Country == GameInfo.PlayerCountry) || defenders.Armies.Any(t => t.Country == GameInfo.PlayerCountry)) {
                BattleResultWindowSpawner.Singleton.Spawn(attackers, defenders, province, killedAttackers, killedDefenders);
            }

            UpdateWarStats();
        }

        private void OnDefendersWin() {
            if (attackers.Armies.Any(t => t.Country == GameInfo.PlayerCountry) || defenders.Armies.Any(t => t.Country == GameInfo.PlayerCountry)) {
                BattleResultWindowSpawner.Singleton.Spawn(defenders, attackers, province, killedDefenders, killedAttackers);
            }

            UpdateWarStats();
        }

        private void UpdateWarStats() {
            ArmyInfo a = attackers.Armies.First();
            ArmyInfo d = defenders.Armies.First();

            if (a.Country.IsInWarAgainstCountry(d.Country)) {
                WarCountryInfo ai = a.Country.wars[a.Country.GetWarAgainstCountry(d.Country)];
                WarCountryInfo di = d.Country.wars[d.Country.GetWarAgainstCountry(a.Country)];
                ai.killedEnemies += killedDefenders;
                ai.killedLocal += killedAttackers;
                di.killedEnemies += killedAttackers;
                di.killedLocal += killedDefenders;

                ai.ChangeWarScore(Mathf.RoundToInt((killedDefenders - killedAttackers) * GameStatistics.battleWarScoreChangeModifier));
                di.ChangeWarScore(Mathf.RoundToInt((killedAttackers - killedDefenders) * GameStatistics.battleWarScoreChangeModifier));
            }
        }
    }
}