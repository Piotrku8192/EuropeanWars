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

        public void CountAttack() {
            foreach (var item in attackers) {
                UnitInfo key = defenders.OrderBy(t => t.Key.type).SingleOrDefault(t => t.Value > 0).Key;
                if (key != null) {
                    defenders[key] = Mathf.Clamp(defenders[key] - Mathf.FloorToInt(
                        item.Value * item.Key.attack * attackersAttackModifier[(int)item.Key.type] / (float)key.health), 0, int.MaxValue);
                }
                else {
                    onAttackersEmpty.Invoke();
                }
            }
            
            foreach (var item in defenders) {
                UnitInfo key = attackers.OrderBy(t => t.Key.type).SingleOrDefault(t => t.Value > 0).Key;
                if (key != null) {
                    attackers[key] = Mathf.Clamp(attackers[key] - Mathf.FloorToInt(
                        item.Value * item.Key.attack * defendersAttackModifier[(int)item.Key.type] / (float)key.health), 0, int.MaxValue);
                }
                else {
                    onDefendersEmpty.Invoke();
                }
            }
        }
    }
}
