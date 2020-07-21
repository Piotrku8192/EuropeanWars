using EuropeanWars.Core.Province;
using EuropeanWars.UI.Windows;
using System;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.Core.Army {
    public class Battle {
        private readonly ArmyInfo attacker;
        private readonly ArmyInfo defender;
        private readonly ProvinceInfo province;
        private readonly ArmyAttackCounter attackCounter;

        private int killedAttackers;
        private int killedDefenders;

        private bool ended;

        public Battle(ArmyInfo attacker, ArmyInfo defender, ProvinceInfo province) {
            this.attacker = attacker;
            this.defender = defender;
            this.province = province;
            attackCounter = new ArmyAttackCounter(attacker.units, defender.units, GameStatistics.battleAttackerArmyAttackModifier,
                GameStatistics.battleDefenderArmyAttackModifier, () => ended = true, () => ended = true);

            PlayBattle();
        }

        private void PlayBattle() {
            int attacksCount = Mathf.CeilToInt((attacker.Size + defender.Size) * GameStatistics.battleAttacksCountModifier);
            for (int i = 0; i < attacksCount; i++) {
                if (ended) {
                    break;
                }

                attackCounter.CountAttack(out int kd, out int ka);
                killedDefenders += kd;
                killedAttackers += ka;
            }

            if (defender.Size <= 0) {
                OnAttackerWin();
                defender.DeleteLocal();
            }
            else if (attacker.Size <= 0) {
                OnDefenderWin();
                attacker.DeleteLocal();
            }
            else if (killedDefenders >= killedAttackers) {
                OnAttackerWin();
                ProvinceInfo[] ps = defender.Country.provinces.Where(t => t != province).ToArray();
                if (ps.Length == 0) {
                    ps = new ProvinceInfo[1] { province.neighbours.First(t => t.isLand && t.isInteractive) };
                }
                defender.GenerateRoute(ps[GameInfo.random.Next(0, ps.Count() - 1)]);
            }
            else if (killedAttackers > killedDefenders) {
                OnDefenderWin();
                ProvinceInfo[] ps = attacker.Country.provinces.Where(t => t != province).ToArray();
                if (ps.Length == 0) {
                    ps = new ProvinceInfo[1] { province.neighbours.First(t => t.isLand && t.isInteractive) };
                }
                attacker.GenerateRoute(ps[GameInfo.random.Next(0, ps.Count() - 1)]);
            }
        }

        private void OnAttackerWin() {
            if (attacker.Country == GameInfo.PlayerCountry || defender.Country == GameInfo.PlayerCountry) {
                BattleResultWindowSpawner.Singleton.Spawn(attacker, defender, province, killedAttackers, killedDefenders);
            }

            if (attacker.Country.IsInWarAgainstCountry(defender.Country)) {
                attacker.Country.wars[attacker.Country.GetWarAgainstCountry(defender.Country)].killedEnemies += killedDefenders;
                attacker.Country.wars[attacker.Country.GetWarAgainstCountry(defender.Country)].killedLocal += killedAttackers;
                defender.Country.wars[defender.Country.GetWarAgainstCountry(attacker.Country)].killedEnemies += killedAttackers;
                defender.Country.wars[defender.Country.GetWarAgainstCountry(attacker.Country)].killedLocal += killedDefenders;
            }
        }

        private void OnDefenderWin() {
            if (attacker.Country == GameInfo.PlayerCountry || defender.Country == GameInfo.PlayerCountry) {
                BattleResultWindowSpawner.Singleton.Spawn(defender, attacker, province, killedDefenders, killedAttackers);
            }

            if (attacker.Country.IsInWarAgainstCountry(defender.Country)) {
                attacker.Country.wars[attacker.Country.GetWarAgainstCountry(defender.Country)].killedEnemies += killedDefenders;
                attacker.Country.wars[attacker.Country.GetWarAgainstCountry(defender.Country)].killedLocal += killedAttackers;
                defender.Country.wars[defender.Country.GetWarAgainstCountry(attacker.Country)].killedEnemies += killedAttackers;
                defender.Country.wars[defender.Country.GetWarAgainstCountry(attacker.Country)].killedLocal += killedDefenders;
            }
        }
    }
}
