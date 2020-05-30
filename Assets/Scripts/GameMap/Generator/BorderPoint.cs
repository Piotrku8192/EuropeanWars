using System;
using System.Numerics;

namespace EuropeanWars.GameMap.Data {
    [Serializable]
    public struct BorderPoint {
        public Vector2 position;
        public Vector2 fixedPosition;
        public string borderProvince;
    }
}
