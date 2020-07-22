using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.Core.Army {
    public class BattleArmyGroup {
        public ArmyInfo[] Armies { get; private set; }

        public int Size => Armies.Sum(t => t.Size);

        public Dictionary<ArmyInfo, Dictionary<UnitInfo, int>> sizeChange;

        private Dictionary<UnitInfo, int> resultUnits = new Dictionary<UnitInfo, int>();
        private Dictionary<UnitInfo, int> startUnits = new Dictionary<UnitInfo, int>();

        public BattleArmyGroup(ArmyInfo[] armies) {
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
            sizeChange = new Dictionary<ArmyInfo, Dictionary<UnitInfo, int>>();

            foreach (var item in resultUnits) {
                if (item.Value != startUnits[item.Key]) {
                    int change = item.Value - startUnits[item.Key];
                    foreach (var a in Armies) {
                        if (!sizeChange.ContainsKey(a)) {
                            sizeChange.Add(a, new Dictionary<UnitInfo, int>());
                        }

                        if (a.units.ContainsKey(item.Key)) {
                            int sChange = Mathf.RoundToInt(change * ((float)a.maxUnits[item.Key]
                                / Armies.Where(t => t.maxUnits.ContainsKey(item.Key)).Sum(t => t.maxUnits[item.Key])));
                            a.units[item.Key] += sChange;

                            if (!sizeChange[a].ContainsKey(item.Key)) {
                                sizeChange[a].Add(item.Key, 0);
                            }
                            sizeChange[a][item.Key] += sChange;
                        }
                    }
                }
            }
        }
    }
}
