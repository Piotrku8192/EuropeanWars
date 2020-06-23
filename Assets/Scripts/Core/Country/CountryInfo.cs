using EuropeanWars.Core.Army;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Data;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Language;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Religion;
using EuropeanWars.Core.Time;
using EuropeanWars.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.Core.Country {
    public class CountryInfo {
        private readonly CountryData data;

        public readonly int id;
        public readonly Color color;

        public bool isPlayer = false;

        //Main
        public string name;
        public Sprite crest;
        public int king; //TODO: Change type when persons added
        public ReligionInfo religion;
        public List<ProvinceInfo> provinces = new List<ProvinceInfo>();

        //Economy
        public int gold;
        public int balance;
        public int taxationIncome;
        public int buildingsIncome;
        public int tradeIncome;
        public float taxationIncomeModifier;
        public float buildingsIncomeModifier;
        public float tradeIncomeModifier;

        //Economy -> Loans
        public int loans;
        public bool isBankruptcy;
        public int monthsToEndBanktruptcy;

        //Army
        public int manpower;
        public int maxManpower;
        public int manpowerIncrease;
        public float manpowerModifier;
        public List<UnitToRecruit> toRecruit = new List<UnitToRecruit>();
        public List<ArmyInfo> armies = new List<ArmyInfo>();

        //Diplomacy
        public int prestige;
        public CountryInfo[] friends;
        public CountryInfo[] enemies;
        public Dictionary<CountryInfo, int> relations = new Dictionary<CountryInfo, int>();
        public Dictionary<CountryInfo, Alliance> alliances = new Dictionary<CountryInfo, Alliance>();

        //Technology and stuff
        public List<BuildingInfo> buildings = new List<BuildingInfo>();
        public List<UnitInfo> units = new List<UnitInfo>();

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
            TimeManager.onDayElapsed += OnDayElapsed;
            TimeManager.onMonthElapsed += OnMonthElapsed;
            TimeManager.onYearElapsed += OnYearElapsed;

            gold = 4000;
            units.AddRange(GameInfo.units.Values); //TODO: Remove this
        }

        public void UpdateLanguage() {
            name = LanguageDictionary.language["CountryName-" + id.ToString()];
        }

        public void OnDayElapsed() {
            TryRecruitUnits();
        }

        public void OnMonthElapsed() {
            CalculateEconomy();
            EndBankruptcy();
            CalculateManpower();
        }

        public void OnYearElapsed() {
            CalculatePrestige();
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

        public void CalculateManpower() {
            //TODO: Update this function after create new system
            maxManpower = taxationIncome * 1000;
            manpowerIncrease = taxationIncome;
            manpowerModifier = 1.0f;
            if (manpower + manpowerIncrease <= maxManpower) {
                manpower += Mathf.FloorToInt(manpowerIncrease * manpowerModifier);
            }
        }

        public void CalculatePrestige() {
            //TODO: Yes, this is definetly bad way to do that.
            prestige += 1;
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
            else if (isBankruptcy) {
                isBankruptcy = false;
                taxationIncomeModifier += 0.2f;
                buildingsIncomeModifier += 0.2f;
                tradeIncomeModifier += 0.2f;
            }
        }

        #endregion

        #region Diplomacy

        //TODO: Move this function to DiplomacyManager.
        public void SetRelationsWithCountry(CountryInfo country, int v) {
            if (relations.ContainsKey(country) && country.relations.ContainsKey(this)) {
                relations[country] = v;
                country.relations[this] = v;
            }
        }

        #endregion

        #region Recrutation
        public void EnqueueUnitToRecruite(UnitInfo info, ProvinceInfo province, int count) {
            if (province.Country == this 
                && manpower >= info.recruitSize * count
                && gold >= info.recruitCost * count
                && province.buildings.Contains(info.recruitBuilding)) {
                UnitToRecruit unit = new UnitToRecruit();
                unit.unitInfo = info;
                unit.province = province;
                unit.country = this;
                unit.days = info.recruitDays;
                unit.count = count;

                UnitToRecruit u = toRecruit.Where(t => t.Equals(unit)).FirstOrDefault();
                if (u != null) {
                    u.count += count;
                }
                else {
                    toRecruit.Add(unit);
                    ArmyWindow.Singleton.recrutationWindow.AddRecruitingUnit(unit);
                }

                manpower -= info.recruitSize * count;
                gold -= info.recruitCost * count;
            }
        }
        public void TryRecruitUnits() {
            List<UnitToRecruit> tr = new List<UnitToRecruit>(toRecruit);
            foreach (UnitToRecruit unit in tr) {
                unit.days -= 1;
                if (unit.days <= 0) {
                    unit.RecruitUnit();
                    toRecruit.Remove(unit);
                }
            }
        }
        #endregion
    }
}
