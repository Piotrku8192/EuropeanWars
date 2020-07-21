using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.Core.Army {
    public class ArmyGroup {
        public ArmyInfo[] Armies { get; private set; }

        public int Size => Armies.Sum(t => t.Size);

        private Dictionary<UnitInfo, int> resultUnits = new Dictionary<UnitInfo, int>();
        private Dictionary<UnitInfo, int> startUnits = new Dictionary<UnitInfo, int>();

        public ArmyGroup(ArmyInfo[] armies) {
            Armies = armies;
        }

        public Dictionary<UnitInfo, int> GetUnits() {
            UpdateArmies();

            resultUnits.Clear();
            startUnits.Clear();

            foreach (var item in Armies) {
                foreach (var u in item.units) {
                    if (resultUnits.ContainsKey(u.Key)) {
                        resultUnits[u.Key] += u.Value;
                    }
                    else {
                        resultUnits.Add(u.Key, u.Value);
                    }
                }
            }

            startUnits = new Dictionary<UnitInfo, int>(resultUnits);
            return resultUnits;
        }

        public void UpdateArmies() {
            foreach (var item in resultUnits) {
                if (item.Value != startUnits[item.Key]) {
                    int change = item.Value - startUnits[item.Key];
                    foreach (var a in Armies) {
                        if (a.units.ContainsKey(item.Key)) {
                            a.units[item.Key] += Mathf.RoundToInt(change * ((float)a.maxUnits[item.Key] 
                                / Armies.Where(t => t.maxUnits.ContainsKey(item.Key)).Sum(t => t.maxUnits[item.Key])));
                        }
                    }
                }
            }
        }
    }
}
