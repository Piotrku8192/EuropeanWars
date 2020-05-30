using EuropeanWars.Core.Building;
using EuropeanWars.Core.Data;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Language;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Religion;
using EuropeanWars.Core.Time;
using System.Collections.Generic;
using UnityEngine;

namespace EuropeanWars.Core.Country {
    public class CountryInfo {
        private readonly CountryData data;

        public readonly int id;
        public readonly Color color;

        public bool isPlayer = false;

        //TODO: Change variables types.
        public string name;
        public Sprite crest;
        public int king;
        public ReligionInfo religion;
        public byte armyPattern;
        public int gold, manpower, prestige, technologyPoints;

        public int balance;
        public int taxationIncome;
        public int buildingsIncome;
        public int tradeIncome;
        public float taxationIncomeModifier;
        public float buildingsIncomeModifier;
        public float tradeIncomeModifier;

        public int loans;
        public bool isBankruptcy;
        public int monthsToEndBanktruptcy;

        public CountryInfo[] friends;
        public CountryInfo[] enemies;
        public Dictionary<CountryInfo, int> relations = new Dictionary<CountryInfo, int>();
        public Dictionary<CountryInfo, Alliance> alliances = new Dictionary<CountryInfo, Alliance>();

        public CountryInfo[] vassals;

        public BuildingInfo[] buildings;
        public int[] squads;
        public int commonUprising;
        public int minUprisingProvinceTax;
        public float[] armyAttackModifiers;
        public float[] armyDefenseModifiers;

        public int[] generals;
        public Dictionary<int, bool> mercenaries = new Dictionary<int, bool>();

        public List<ProvinceInfo> provinces = new List<ProvinceInfo>();

        public CountryInfo(CountryData data) {
            this.data = data;
            id = data.id;
            color = DataConverter.ToColor(int.Parse(data.color, System.Globalization.NumberStyles.HexNumber));

            taxationIncomeModifier = data.taxationIncomeModifier;
            buildingsIncomeModifier = data.buildingsIncomeModifier;
            tradeIncomeModifier = data.tradeIncomeModifier;
        }

        public void Initialize() {
            crest = GameInfo.gfx["Country-" + id];
            religion = GameInfo.religions[data.religion];
            UpdateLanguage();
            TimeManager.onMonthElapsed += OnMonthElapsed;

            gold = 4000;
        }

        public void UpdateLanguage() {
            name = LanguageDictionary.language["CountryName-" + id.ToString()];
        }

        public void OnMonthElapsed() {
            CalculateEconomy();
            EndBankruptcy();
        }

        public void CalculateEconomy() {
            int lastGold = gold;

            //Calculate income
            taxationIncome = 0;
            buildingsIncome = 0;
            tradeIncome = 0;
            foreach (var item in provinces) {
                taxationIncome += item.taxation;
                buildingsIncome += item.buildingsIncome;
                tradeIncome += item.tradeIncome;
            }
            taxationIncome = Mathf.FloorToInt(taxationIncome * taxationIncomeModifier);
            buildingsIncome = Mathf.FloorToInt(buildingsIncome * buildingsIncomeModifier);
            tradeIncome = Mathf.FloorToInt(tradeIncome * tradeIncomeModifier);

            gold += taxationIncome + buildingsIncome + tradeIncome;

            //Calculate maintenance and other stuff.

            if (!isBankruptcy && gold < 0) {
                TakeLoans(gold / -300);
            }
            if (loans > 40) {
                Bankruptcy();
            }
            balance = gold - lastGold;
        }

        #region Loans

        public void TakeLoans(int count = 1) {
            for (int i = 0; i < count; i++) {
                gold += 300;
                loans++;
            }
        }

        public void PayOffLoans(int count = 1) {
            for (int i = 0; i < count; i++) {
                if (loans == 0 || gold < 300) {
                    break;
                }

                gold -= 300;
                loans--;
            }
        }

        public void Bankruptcy() {
            monthsToEndBanktruptcy = 120;
            loans = 0;
            isBankruptcy = true;
            taxationIncomeModifier -= 0.2f;
            buildingsIncomeModifier -= 0.2f;
            tradeIncomeModifier -= 0.2f;
        }

        public void EndBankruptcy() {
            if (monthsToEndBanktruptcy > 0) {
                monthsToEndBanktruptcy--;
            }
            else {
                isBankruptcy = false;
                taxationIncomeModifier += 0.2f;
                buildingsIncomeModifier += 0.2f;
                tradeIncomeModifier += 0.2f;
            }
        }

        #endregion

        #region Diplomacy

        public void SetRelationsWithCountry(CountryInfo country, int v) {
            if (relations.ContainsKey(country) && country.relations.ContainsKey(this)) {
                relations[country] = v;
                country.relations[this] = v;
            }
        }

        #endregion
    }
}
