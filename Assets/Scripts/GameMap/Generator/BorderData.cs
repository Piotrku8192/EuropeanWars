using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace EuropeanWars.GameMap.Data {
    [Serializable]
    public class BorderData {
        public string firstProvince;
        public string secondProvince;
        public List<List<Vector2>> vertices = new List<List<Vector2>>();
    }
}
