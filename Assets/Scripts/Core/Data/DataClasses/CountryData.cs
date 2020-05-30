using System;
using System.Collections.Generic;

namespace EuropeanWars.Core.Data {
    [Serializable]
    public class CountryData {
        public string color;
        public string crest;
        public int id;
        public int king;
        public int[] friends;
        public int[] enemies;
        public byte religion;
        public byte armyPattern;
        public int gold, manpower, prestige, technologyPoints;
        public int[] buildings;
        public int[] squads;
        public int commonUprising;
        public int minUprisingProvinceTax;
        public float taxationIncomeModifier;
        public float buildingsIncomeModifier;
        public float tradeIncomeModifier;
        public float[] armyAttackModifiers;
        public float[] armyDefenseModifiers;
        public int loans;
        public bool isBankruptcy;
        public Dictionary<int, int> relations = new Dictionary<int, int>();
        public Dictionary<int, int> truces = new Dictionary<int, int>();
        public int[] vassals;
        public int[] generals;
        public Dictionary<int, bool> mercenaries = new Dictionary<int, bool>();
    }
}
