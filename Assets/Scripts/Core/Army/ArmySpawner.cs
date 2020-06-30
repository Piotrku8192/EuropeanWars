using UnityEngine;

namespace EuropeanWars.Core.Pathfinding {
    public class ArmySpawner : MonoBehaviour {
        public static ArmySpawner Singleton { get; private set; }

        public ArmyObject armyObjectPrefab;

        public void Awake() {
            Singleton = this;
        }

        public ArmyObject SpawnAndInitializeArmy(ArmyInfo army) {
            ArmyObject a = Instantiate(armyObjectPrefab, transform);
            a.Initialize(army);
            return a;
        }

        public void DestroyArmy(ArmyObject army) {
            Destroy(army.gameObject);
        }
    }
}
