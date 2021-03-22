namespace EuropeanWars.Core {
    public static class GameStatistics {
        public static float armyAverageAttackModifier = 260.0f;
        public static float[] occupantArmyAttackModifier = new float[4] { 0.7f, 0.3f, 0.9f, 0.1f };
        public static float[] occupatedArmyAttackModifier = new float[4] { 1.0f, 0.2f, 1.1f, 0.1f };
        public static int maxArtilleryOccupationBonus = 50;
        public static float artilleryOccupationBonusModifier = 0.03f;
        public static float battleAttacksCountModifier = 0.01f;
        public static float[] battleAttackerArmyAttackModifier = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };
        public static float[] battleDefenderArmyAttackModifier = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };
        public static float battleWarScoreChangeModifier = 0.003f;
        public static int provinceIncomeArmyReinforcementModifier = 10;
        public static float armySpeedModifier = 0.8f;
    }
}
