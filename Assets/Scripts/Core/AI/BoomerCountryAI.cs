using EuropeanWars.Core.Army;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.War;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.Core.AI {
    public class BoomerCountryAI : CountryAI {
        public BoomerCountryAI(CountryInfo country) : base(country) {
        }

        protected override void OnDayElapsed() {
            //throw new NotImplementedException();
            MergeArmies();
        }

        protected override void OnMonthElapsed() {
            BuildBuildings();
            RecruitArmies();
            MakeClaims();
            MoveArmies();
            DeclareWars();
        }

        protected override void OnYearElapsed() {
            //throw new NotImplementedException();
        }

        private void BuildBuildings() {
            if (country.gold > 500) {
                var ps = country.provinces.Where(t => t.buildings.Contains(GameInfo.buildings[0]));
                if (ps.Count() == 0) {
                    return;
                }

                ProvinceInfo p = ps.OrderBy(t => t.taxation).Last();
                BuildingInfo[] b = country.buildings.Where(t => t.cost <= country.gold && t.id != 0).ToArray();

                int slot = 0;
                for (int i = 0; i < p.buildings.Length; i++) {
                    if (p.buildings[i].id == 0) {
                        slot = i;
                        break;
                    }
                }
                if (b.Count() > 0) {
                    BuildBuildingInSlot(p.buildings.Any(t => t.id == 1) ? b[GameInfo.random.Next(0, b.Count() - 1)] : GameInfo.buildings[1], p, slot);
                }
            }
        }

        private void RecruitArmies() {
            if (country.gold > 2000) {
                UnitInfo u = country.units[GameInfo.random.Next(0, country.units.Count - 1)];
                ProvinceInfo[] ps = country.claimedProvinces.Where(t => t.buildings.Contains(u.recruitBuilding)).ToArray();
                if (ps.Length > 0) {
                    int count = Mathf.Min(
                        new int[3] {
                        country.manpower / (u.recruitSize == 0 ? 1 : u.recruitSize),
                        Mathf.FloorToInt(country.balance / (u.maintenance == 0 ? 1 : u.maintenance * u.recruitSize)),
                        country.gold / (u.recruitCost == 0 ? 1 : u.recruitCost)
                        });
                    if (count > 0) {
                        RecruitArmy(u,
                            ps[GameInfo.random.Next(0, ps.Length - 1)],
                            count);
                    }
                }
            }
        }

        private void MergeArmies() {
            List<ProvinceInfo> provinces = new List<ProvinceInfo>();
            foreach (var item in country.armies) {
                if (!provinces.Contains(item.Province)) {
                    provinces.Add(item.Province);
                }
            }

            foreach (var item in provinces) {
                item.MergeArmiesRequest(item.armies.Where(t => t.Country == country).ToArray());
            }
        }

        private void MoveArmies() {
            List<ProvinceInfo> occupatedProvinces = country.nationalProvinces.Where(t => 
            !country.provinces.Contains(t) || (t.OccupationCounter.Progress > 0 && t.OccupationCounter.Army?.Country != country)).ToList();

            List<ProvinceInfo> targetProvinces = new List<ProvinceInfo>();

            if (country.wars.Count > 0) {
                foreach (var item in country.wars) {
                    foreach (var enemy in item.Value.party.Enemies.countries) {
                        targetProvinces.AddRange(enemy.Key.provinces.Where(t => t.neighbours.Any(x => x.Country == country)));
                    }
                }
            }

            foreach (var item in country.armies) {
                if (item.isMoveLocked || item.Province.OccupationCounter.Army == item) {
                    continue;
                }

                if (item.BlackStatus) {
                    item.GenerateRouteRequest(country.provinces[GameInfo.random.Next(0, country.provinces.Count)]);//TODO: Add province validation plz...
                }
                else if (occupatedProvinces.Count > 0) {
                    item.GenerateRouteRequest(occupatedProvinces[0]);
                    occupatedProvinces.RemoveAt(0);
                }
                else if (targetProvinces.Count > 0) {
                    item.GenerateRouteRequest(targetProvinces[0]);
                    targetProvinces.RemoveAt(0);
                }
            }
        }

        private void MakeClaims() {
            int i = 0;
            foreach (var item in country.nationalProvinces) {
                foreach (var n in item.neighbours) {
                    if (n.isLand && n.isInteractive && !country.friends.Contains(n.NationalCountry)) {
                        if (i > country.maxClaimsAtOneTime) {
                            return;
                        }
                        FabricateClaim(n);
                        i++;
                    }
                }
            }
        }

        private void DeclareWars() { //TEMPORARY!!!
            foreach (var item in country.claimedProvinces) {
                if (!country.IsInWarAgainstCountry(item.NationalCountry) && country.manpower > item.NationalCountry.manpower && country.wars.Count < 5) {
                    DeclareWar(item.NationalCountry, 0); //TODO: Change it to something better.
                }
            }
        }
    }
}
