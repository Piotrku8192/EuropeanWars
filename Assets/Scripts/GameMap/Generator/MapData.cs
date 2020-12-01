using System;
using System.Collections.Generic;

namespace EuropeanWars.GameMap.Data {
    [Serializable]
    public class MapData {
        public List<MapProvinceData> provinces = new List<MapProvinceData>();
        public List<BorderData> borders = new List<BorderData>();
    }
}
