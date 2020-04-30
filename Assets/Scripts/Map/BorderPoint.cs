using System;
using System.Numerics;

namespace MapBuilder {
    [Serializable]
    public struct BorderPoint {
        public Vector2 position;
        public Vector2 fixedPosition;
        public string borderProvince;
    }
}
