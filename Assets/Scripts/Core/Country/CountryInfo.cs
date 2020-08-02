using EuropeanWars.Core.Army;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Data;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Language;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Religion;
using EuropeanWars.Core.Time;
using EuropeanWars.Core.War;
using EuropeanWars.UI;
using EuropeanWars.UI.Windows;
using System;
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
		public float manpowerModifier;
		public List<UnitToRecruit> toRecruit = new List<UnitToRecruit>();
		public List<ArmyInfo> armies = new List<ArmyInfo>();

		//Diplomacy
		public int prestige;
		public List<CountryInfo> friends = new List<CountryInfo>();
		public List<CountryInfo> enemies = new List<CountryInfo>();
		public Dictionary<CountryInfo, int> relations = new Dictionary<CountryInfo, int>();
		public Dictionary<WarInfo, WarCountryInfo> wars = new Dictionary<WarInfo, WarCountryInfo>();
		public Dictionary<CountryInfo, Alliance> alliances = new Dictionary<CountryInfo, Alliance>();
		public Dictionary<CountryInfo, MilitaryAccess> militaryAccesses= new Dictionary<CountryInfo, MilitaryAccess>();

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
			crest = GameInfo.gfx["Country-" + id];
			religion = GameInfo.religions[data.religion];
			buildings = GameInfo.buildings.Values.ToList();
			UpdateLanguage();
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
		}
		public void UpdateLanguage() {
			name = LanguageDictionary.language["CountryName-" + id.ToString()];
		}

		public void OnDayElapsed() {
			TryRecruitUnits();
			TryFabricateClaim();
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
			manpowerModifier = 0.6f;
			if (manpower + manpowerIncrease <= maxManpower) {
				manpower += Mathf.FloorToInt(manpowerIncrease * manpowerModifier);
			}
		}
		public void CalculatePrestige() {
			//TODO: Yes, this is definetly bad way to do that.
			prestige += 1;
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
	}
}
