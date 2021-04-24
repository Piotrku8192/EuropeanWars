using System;

namespace EuropeanWars.GameMap.Data {
    [Serializable]
    public class MapProvinceData {
        public string color;
        public MeshData mesh = new MeshData();
    }
}
