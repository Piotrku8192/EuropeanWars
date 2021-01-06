using EuropeanWars.Core.Army;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Data;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Language;
using EuropeanWars.Core.Persons;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Religion;
using EuropeanWars.Core.Time;
using EuropeanWars.Core.War;
using EuropeanWars.UI;
using EuropeanWars.UI.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.Core.Country {
    public class CountryInfo {
        private readonly CountryData data;

        public readonly int id;
        public readonly Color color;

        public bool isPlayer = false;

        //Main
        public string Name { get; private set; }
        public Sprite Crest { get; private set; }
        public int king; //TODO: Change type when persons added
        public ReligionInfo religion;
        public List<ProvinceInfo> provinces = new List<ProvinceInfo>();
        public List<ProvinceInfo> nationalProvinces = new List<ProvinceInfo>();

        public int maxClaimsAtOneTime = 2;
        public Dictionary<ProvinceInfo, int> toClaim = new Dictionary<ProvinceInfo, int>();
        public List<ProvinceInfo> claimedProvinces = new List<ProvinceInfo>();

        //Economy
        public int gold;
        public int balance;
        public int taxationIncome;
        public int buildingsIncome;
        public int tradeIncome;
        public int armyMaintenance;
        public float taxationIncomeModifier;
        public float buildingsIncomeModifier;
        public float tradeIncomeModifier;
        public float armyMaintenanceModifier;

        //Economy -> Loans
        public int loans;
        public bool isBankruptcy;
        public int monthsToEndBanktruptcy;

        //Army
        public int manpower;
        public int maxManpower;
        public int manpowerIncrease;
        public float manpowerIncreaseModifier;
        public List<UnitToRecruit> toRecruit = new List<UnitToRecruit>();
        public List<ArmyInfo> armies = new List<ArmyInfo>();

        //Diplomacy
        public int prestige;
        public List<CountryInfo> friends = new List<CountryInfo>();
        public List<CountryInfo> enemies = new List<CountryInfo>();
        public Dictionary<WarInfo, WarCountryInfo> wars = new Dictionary<WarInfo, WarCountryInfo>();
        public Dictionary<CountryInfo, CountryRelation> relations = new Dictionary<CountryInfo, CountryRelation>();

        public List<Diplomat> diplomats = new List<Diplomat>();
        public List<Spy> spies = new List<Spy>();

        //Vassals
        public bool isVassal;

        //if country is vassal
        public CountryInfo suzerain;
        /// <summary>
        /// if false country canot make alliances, wars and so one.
        /// </summary>
        public bool sovereign = true;
        public bool isMarchy;

        //if country is not vassal
        public List<CountryInfo> vassals = new List<CountryInfo>();

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
            armyMaintenanceModifier = 1;
        }

        public void Initialize() {
            Crest = GameInfo.gfx["Country-" + id];
            religion = GameInfo.religions[data.religion];
            buildings = GameInfo.buildings.Values.ToList();
            TimeManager.onDayElapsed += OnDayElapsed;
            TimeManager.onMonthElapsed += OnMonthElapsed;
            TimeManager.onYearElapsed += OnYearElapsed;

            gold = 40000;
            foreach (var item in data.squads) {
                units.Add(GameInfo.units[item]);
            }

            foreach (var item in data.friends) {
                friends.Add(GameInfo.countries[item]);
            }
            foreach (var item in data.enemies) {
                enemies.Add(GameInfo.countries[item]);
            }

            foreach (var item in GameInfo.countries) {
                if (item.Value != this && item.Value.id > 0) {
                    if (!item.Value.relations.ContainsKey(this)) {
                        CountryRelation relation = new CountryRelation(GameInfo.random.Next(-100, 100)); //TODO: Change it to something else
                        relations.Add(item.Value, relation);
                        item.Value.relations.Add(this, relation);
                    }
                }
            }

            foreach (var item in data.vassals) {
                MakeVassal(GameInfo.countries[item]);
            }

            //TODO: Remove it
            diplomats.Add(new Diplomat("Diplomat1", 1609, 1700, this, 0.4f));
            diplomats.Add(new Diplomat("Diplomat2", 1609, 1700, this, 0.4f));
            diplomats.Add(new Diplomat("Diplomat3", 1609, 1700, this, 0.4f));
            spies.Add(new Spy("Spy1", 1609, 1700, this, 2));
            spies.Add(new Spy("Spy2", 1609, 1700, this, 2));
            spies.Add(new Spy("Spy3", 1609, 1700, this, 2));
        }
        public void UpdateLanguage() {
            Name = LanguageDictionary.language["CountryName-" + id.ToString()];
        }

        public void OnDayElapsed() {
            TryRecruitUnits();
            TryFabricateClaim();
        }
        public void OnMonthElapsed() {
            CalculateEconomy();
            EndBankruptcy();
            CalculateManpower();
            CalculateDiplomacy();
        }
        public void OnYearElapsed() {
            CalculatePrestige();
        }

        public void CalculateEconomy() {
            int lastGold = gold;

            //Calculate income
            CalculateTradeModifier();

            taxationIncome = 0;
            buildingsIncome = 0;
            tradeIncome = 0;
            armyMaintenance = 0;
            foreach (var item in provinces) {
                taxationIncome += item.taxation;
                buildingsIncome += item.buildingsIncome;
                tradeIncome += item.tradeIncome;
            }
            taxationIncome = Mathf.FloorToInt(taxationIncome * taxationIncomeModifier);
            buildingsIncome = Mathf.FloorToInt(buildingsIncome * buildingsIncomeModifier);
            tradeIncome = Mathf.FloorToInt(tradeIncome * tradeIncomeModifier);

            foreach (var item in armies) {
                armyMaintenance += item.Maintenance;
            }
            armyMaintenance = Mathf.FloorToInt(armyMaintenance * armyMaintenanceModifier);

            gold += taxationIncome + buildingsIncome + tradeIncome - armyMaintenance;

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
            maxManpower = nationalProvinces.Count * 5000;
            manpowerIncrease = taxationIncome;
            manpowerIncreaseModifier = 0.6f;
            if (manpower + manpowerIncrease <= maxManpower) {
                manpower += Mathf.FloorToInt(manpowerIncrease * manpowerIncreaseModifier);
            }
        }
        public void CalculatePrestige() {
            //TODO: Yes, this is definetly bad way to do that.
            prestige += 1;
        }

        public void CalculateDiplomacy() {
            if (DiplomacyWindow.Singleton.countryWindow.country == this && DiplomacyWindow.Singleton.window.activeInHierarchy) {
                DiplomacyWindow.Singleton.UpdateWindow();
            }
        }

        public void EnqueFabricateClaim(ProvinceInfo province) {
            if (!claimedProvinces.Contains(province) && !toClaim.ContainsKey(province)
                && province.neighbours.Where(t => t.NationalCountry == this).Any() && province.isInteractive
                && !IsInWarAgainstCountry(province.NationalCountry)) {
                toClaim.Add(province, province.taxation * 10);

                if (ProvinceWindow.Singleton.province == province) {
                    ProvinceWindow.Singleton.UpdateWindow(province);
                }
            }
        }
        private void TryFabricateClaim() {
            for (int i = 0; i < maxClaimsAtOneTime; i++) {
                if (toClaim.Count > i) {
                    ProvinceInfo key = toClaim.Keys.ToArray()[i];
                    toClaim[key]--;
                }
                else {
                    break;
                }
            }

            foreach (var item in new Dictionary<ProvinceInfo, int>(toClaim)) {
                if (item.Value <= 0) {
                    item.Key.FabricateClaim(this);
                    toClaim.Remove(item.Key);
                    if (ProvinceWindow.Singleton.province == item.Key) {
                        ProvinceWindow.Singleton.UpdateWindow(item.Key);
                    }
                }
            }
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
            if (!isBankruptcy) {
                monthsToEndBanktruptcy = 60;
                loans = 0;
                isBankruptcy = true;
                taxationIncomeModifier -= 0.2f;
                buildingsIncomeModifier -= 0.2f;
                tradeIncomeModifier -= 0.2f;
            }
        }
        private void EndBankruptcy() {
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

        #region Trade
        private void CalculateTradeModifier() {
            int totalCities = GameInfo.provinces.Where(t => t.Value.isTradeCity).Count();
            int cities = provinces.Where(t => t.isTradeCity).Count();
            float f = cities / (float)totalCities;
            float tradeCitiesModifier = 0;

            //TODO: Change static values to settings in GameSettings
            if (f >= 0.01f && f < 0.05f)
                tradeCitiesModifier = 0.05f;
            else if (f >= 0.05f && f < 0.12f)
                tradeCitiesModifier = 0.1f;
            else if (f >= 0.12f && f < 0.25f)
                tradeCitiesModifier = 0.2f;
            else if (f >= 0.25f && f < 0.45f)
                tradeCitiesModifier = 0.5f;
            else if (f >= 0.45f && f < 0.75f)
                tradeCitiesModifier = 1f;
            else if (f >= 0.75f)
                tradeCitiesModifier = 2f;

            tradeIncomeModifier = 1;
            tradeIncomeModifier += tradeCitiesModifier;
            tradeIncomeModifier += cities * 0.01f;
            tradeIncomeModifier += provinces.Where(t => t.isTradeRoute).Count() * 0.005f;
        }
        #endregion

        #region Recrutation
        public void EnqueueUnitToRecruit(UnitInfo info, ProvinceInfo province, int count) {
            if (count > 0
                && province.Country == this
                && manpower >= info.recruitSize * count
                && gold >= info.recruitCost * count
                && province.buildings.Contains(info.recruitBuilding)
                && claimedProvinces.Contains(province)) {
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
                    if (GameInfo.PlayerCountry == this) {
                        ArmyWindow.Singleton.recrutationWindow.AddRecruitingUnit(unit);
                    }
                }

                manpower -= info.recruitSize * count;
                gold -= info.recruitCost * count;
            }
        }
        private void TryRecruitUnits() {
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

        #region War
        public bool IsInWarAgainstCountry(CountryInfo country) {
            foreach (var item in wars) {
                if (item.Value.party.Enemies.ContainsCountry(country)) {
                    return true;
                }
            }
            return false;
        }
        public bool IsInWarWithCountry(CountryInfo country) {
            foreach (var item in wars) {
                if (item.Key.ContainsCountry(country)) {
                    return true;
                }
            }
            return false;
        }
        public WarInfo GetWarAgainstCountry(CountryInfo country) {
            if (IsInWarAgainstCountry(country)) {
                foreach (var item in wars) {
                    if (item.Value.party.Enemies.ContainsCountry(country)) {
                        return item.Key;
                    }
                }
            }

            return null;
        }
        #endregion

        #region Vassals
        public bool CanMakeVassal(CountryInfo country, bool isInPeaceDeal = false) {
            return country.suzerain != this && !isVassal && country.taxationIncome * 2 < taxationIncome
                && relations[country].Points == 100
                && relations[country].truceInMonths == 0
                && (isInPeaceDeal || (wars.Count == 0 && country.wars.Count == 0));
        }

        public bool CanAnnexVassal(CountryInfo country) {
            return country.suzerain == this && isVassal && country.taxationIncome * 3 < taxationIncome
                && relations[country].Points == 100
                && relations[country].truceInMonths == 0
                && wars.Count == 0 && country.wars.Count == 0;
        }

        public void MakeVassal(CountryInfo country) {
            if (country.suzerain != this) {
                if (country.isVassal) {
                    country.suzerain.RemoveMarchy(country);
                    country.suzerain.vassals.Remove(country);
                }
                country.suzerain = this;
                country.isVassal = true;
                country.sovereign = true;
                vassals.Add(country);

                foreach (var item in country.wars) {
                    if (!wars.Keys.Contains(item.Key)) {
                        item.Key.JoinWar(this, item.Value.party == item.Key.attackers);
                        if (item.Value.IsMajor) {
                            item.Value.party.major = wars[item.Key];
                        }
                    }
                }

                if (this == GameInfo.PlayerCountry || country == GameInfo.PlayerCountry) {
                    foreach (var item in provinces) {
                        item.RefreshFogOfWarInRegion();
                    }
                    foreach (var item in country.provinces) {
                        item.RefreshFogOfWarInRegion();
                    }
                }
            }
        }

        public void AnnexVassal(CountryInfo country) {
            if (country.suzerain == this) {
                foreach (var item in country.armies.ToArray()) {
                    item.SetCountry(this);
                }

                foreach (var item in country.provinces.ToArray()) {
                    item.SetCountry(this, item.NationalCountry == country);
                }
            }
        }

        public void RemoveVassal(CountryInfo country) {
            if (country.suzerain == this) {
                country.isVassal = false;
                country.suzerain = null;
                country.sovereign = true;
                vassals.Remove(country);
            }
        }

        public void MakeMarchy(CountryInfo country) {
            if (country.suzerain == this && !country.isMarchy) {
                //TODO: Add effects
                country.isMarchy = true;
            }
        }

        public void RemoveMarchy(CountryInfo country) {
            if (country.suzerain == this && country.isMarchy) {
                //TODO: Remove effects
                country.isMarchy = false;
            }
        }
        #endregion

        public void OnCountryClearedFromMap() {
            if (nationalProvinces.Count == 0) {
                foreach (var item in wars.ToArray()) {
                    PeaceDeal p = new PeaceDeal(item.Key, item.Value, item.Value.party.Enemies.major);
                    p.Execute();
                }

                foreach (var item in provinces.ToArray()) {
                    item.SetCountry(item.NationalCountry, false);
                }

                foreach (var item in relations) {
                    for (int i = 0; i < item.Value.relations.Length; i++) {
                        if (item.Value.relations[i]) {
                            item.Value.ChangeRelationState(i, this, item.Key);
                        }
                    }
                }

                foreach (var item in armies.ToArray()) {
                    item.DeleteLocal();
                }

                loans = 0;
                toClaim.Clear();

                sovereign = false;
                isVassal = false;
                if (suzerain != null) {
                    suzerain.RemoveMarchy(this);
                    suzerain.vassals.Remove(this);
                    suzerain = null;
                }
                vassals.Clear();
            }
        }

        #region Diplomacy
        public Diplomat GetDiplomatInRelation(CountryRelation relation) {
            if (relation == null) {
                return null;
            }

            foreach (Diplomat item in diplomats) {
                if (item.CurrentlyImprovingRelation == relation) {
                    return item;
                }
            }

            return null;
        }
        public Spy GetSpyInRelation(CountryRelation relation) {
            if (relation == null) {
                return null;
            }

            foreach (Spy item in spies) {
                if (item.CurrentlyBuildingSpyNetwork == relation) {
                    return item;
                }
            }

            return null;
        }
        #endregion
    }
}
