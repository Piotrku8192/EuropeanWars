using EuropeanWars.Core.War;
using System.Collections.Generic;
using UnityEngine;

namespace EuropeanWars.UI.Windows {
    public class WarList : MonoBehaviour {
        public static WarList Singleton { get; private set; }

        public WarButton warButtonPrefab;
        private Dictionary<WarCountryInfo, WarButton> wars = new Dictionary<WarCountryInfo, WarButton>();

        public void Awake() {
            Singleton = this;
        }

        public void AddWar(WarCountryInfo warCountryInfo) {
            WarButton go = Instantiate(warButtonPrefab, transform);
            go.Init(warCountryInfo);
            wars.Add(warCountryInfo, go);
        }

        public void RemoveWar(WarCountryInfo warCountryInfo) {
            if (wars.ContainsKey(warCountryInfo)) {
                Destroy(wars[warCountryInfo].gameObject);
                wars.Remove(warCountryInfo);
            }
        }
    }
}
