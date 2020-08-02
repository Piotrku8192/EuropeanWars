using EuropeanWars.Core.Army;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Province;
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
                    p?.BuildBuilding(p.buildings.Any(t => t.id == 1) ? b[GameInfo.random.Next(0, b.Count() - 1)] : GameInfo.buildings[1], slot);
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
                        country.EnqueueUnitToRecruit(u,
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
                item.MergeArmies(item.armies.Where(t => !t.isMoveLocked).ToArray());
            }
        }

        private void MakeClaims() {
            foreach (var item in country.nationalProvinces) {
                foreach (var n in item.neighbours) {
                    if (!country.friends.Contains(n.NationalCountry) && !country.claimedProvinces.Contains(n)) {
                        country.EnqueFabricateClaim(n);
                    }
                }
            }
        }
    }
}
