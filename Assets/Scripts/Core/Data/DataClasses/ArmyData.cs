namespace EuropeanWars.Core.Data {
    [System.Serializable]
    public class ArmyData {
        public bool isNavy;
        public string province;
        public int country;
        public int[] unitsT;
        public int[] unitsS;
        public int[] maxUnitsT;
        public int[] maxUnitsS;
        public string[] route;
        public bool blackStatus;
        public bool isMovingLocked;
    }
}
