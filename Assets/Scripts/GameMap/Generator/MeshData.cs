using System;
using System.Collections.Generic;
using System.Numerics;

namespace EuropeanWars.GameMap.Data {
    [Serializable]
    public class MeshData {
        public List<Vector2> vertices = new List<Vector2>();
        public List<int> indices = new List<int>();
    }
}
