using EuropeanWars.Core.War;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    [RequireComponent(typeof(Outline))]
    public class WarReasonButton : MonoBehaviour {
        public Text reasonName;
        public WarReason warReason;
        public int warReasonId;
        public DeclareWarWindow declareWarWindow;

        public void OnClick() {
            GetComponent<Outline>().enabled = true;
            declareWarWindow.selectedReason = this;
            declareWarWindow.selectedReason.GetComponent<Outline>().enabled = false;
        }
    }
}
