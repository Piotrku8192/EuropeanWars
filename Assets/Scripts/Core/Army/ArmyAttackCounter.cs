using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.Core.Army {
    public class ArmyAttackCounter {
        public delegate void OnAttackersEmpty();
        public delegate void OnDefendersEmpty();

        private readonly Dictionary<UnitInfo, int> attackers;
        private readonly Dictionary<UnitInfo, int> defenders;
        private readonly float[] attackersAttackModifier;
        private readonly float[] defendersAttackModifier;
        private readonly OnAttackersEmpty onAttackersEmpty;
        private readonly OnDefendersEmpty onDefendersEmpty;

        public ArmyAttackCounter(Dictionary<UnitInfo, int> attackers, Dictionary<UnitInfo, int> defenders, float[] attackersAttackModifier,
            float[] defendersAttackModifier, OnAttackersEmpty onAttackersEmpty, OnDefendersEmpty onDefendersEmpty) {
            this.attackers = attackers;
            this.defenders = defenders;
            this.attackersAttackModifier = attackersAttackModifier;
            this.defendersAttackModifier = defendersAttackModifier;
            this.onAttackersEmpty = onAttackersEmpty;
            this.onDefendersEmpty = onDefendersEmpty;
        }

        public void CountAttack(out int killedDefenders, out int killedAttackers) {
            killedDefenders = 0;
            killedAttackers = 0;

            int attackersAttack = Mathf.RoundToInt(attackers.Sum(t => t.Key.attack * t.Value
            * attackersAttackModifier[(int)t.Key.type]) / attackers.Sum(t => t.Value) * GameStatistics.armyAverageAttackModifier);
            int defendersAttack = Mathf.RoundToInt(defenders.Sum(t => t.Key.attack * t.Value
            * defendersAttackModifier[(int)t.Key.type]) / defenders.Sum(t => t.Value) * GameStatistics.armyAverageAttackModifier);

            Dictionary<UnitInfo, int> defendersToKill = new Dictionary<UnitInfo, int>();
            Dictionary<UnitInfo, int> attackersToKill = new Dictionary<UnitInfo, int>();

            foreach (var item in defenders.OrderBy(t => t.Key.type).ThenBy(t => t.Key.id)) {
                if (attackersAttack <= 0) {
                    break;
                }

                int k = Mathf.Clamp(attackersAttack / item.Key.health, 0, item.Value);
                attackersAttack -= item.Key.health * k;
                killedDefenders += k;
                if (k > 0) {
                    defendersToKill.Add(item.Key, k);
                }
            }

            foreach (var item in attackers.OrderBy(t => t.Key.type).ThenBy(t => t.Key.id)) {
                if (defendersAttack <= 0) {
                    break;
                }

                int k = Mathf.Clamp(defendersAttack / item.Key.health, 0, item.Value);
                defendersAttack -= item.Key.health * k;
                killedAttackers += k;
                if (k > 0) {
                    attackersToKill.Add(item.Key, k);
                }
            }

            foreach (var item in attackersToKill) {
                attackers[item.Key] -= item.Value;
            }
            foreach (var item in defendersToKill) {
                defenders[item.Key] -= item.Value;
            }
            if (!attackers.Where(t => t.Value > 0).Any()) {
                onAttackersEmpty.Invoke();
            }
            if (!defenders.Where(t => t.Value > 0).Any()) {
                onDefendersEmpty.Invoke();
            }
        }
    }
}
