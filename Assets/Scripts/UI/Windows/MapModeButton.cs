using EuropeanWars.GameMap;
using UnityEngine;

namespace EuropeanWars.UI.Windows {
    public class MapModeButton : MonoBehaviour {
        public MapMode mapMode;

        public void OnClick() {
            MapPainter.PaintMap(mapMode);
        }
    }
}
