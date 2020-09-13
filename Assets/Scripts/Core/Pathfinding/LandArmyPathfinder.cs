using EuropeanWars.Core.Army;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Province;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Roy_T.AStar.Graphs;
using Roy_T.AStar.Paths;

namespace EuropeanWars.Core.Pathfinding {
    public class LandArmyPathfinder : IPathfinder {
        private readonly ArmyInfo army;
        private readonly CountryInfo country;


        public LandArmyPathfinder(ArmyInfo army) {
            this.army = army;
            this.country = army.Country;
        }

        public ProvinceInfo[] FindPath(ProvinceInfo province) {
            foreach (var item in GameInfo.provinces) {
                item.Value.node.Movable = IsMoveable(item.Value);
            }

            if (!IsMoveable(province)) {
                return null;
            }

            PathFinder pathFinder = new PathFinder();
            Path p = pathFinder.FindPath(army.Province.node, province.node, Roy_T.AStar.Primitives.Velocity.FromMetersPerSecond(1));

            ProvinceInfo[] path = new ProvinceInfo[p.Edges.Count + 1];
            path[0] = army.Province;
            for (int i = 1; i < p.Edges.Count + 1; i++) {
                path[i] = GameInfo.provinces.First(t => t.Value.node == p.Edges[i - 1].End).Value;
            }

            //ProvinceInfo[] path = GetPath((new ProvinceInfo[1] { army.Province }, 0), province, 10).Item1;

            return path;
        }

        private (ProvinceInfo[], int) GetPath((ProvinceInfo[], int) path, ProvinceInfo end, int hops) {
            if (hops == 0) {
                return (new ProvinceInfo[0], 0);
            }

            List<(int, ProvinceInfo)> provinces = new List<(int, ProvinceInfo)>();

            foreach (var item in path.Item1.Last().neighbours) {
                if (IsMoveable(item) && !path.Item1.Contains(item)) {
                    int c = path.Item2 + //Vector2.Distance(new Vector2(path.Item1.Last().x, path.Item1.Last().y), new Vector2(item.x, item.y)) +
                    (int)Vector2.Distance(new Vector2(item.x, item.y), new Vector2(end.x, end.y));

                    if (item == end) {
                        return (new List<ProvinceInfo>(path.Item1) { item }.ToArray(), path.Item2 + c); 
                    }

                    provinces.Add((c, item));

                }
            }

            if (provinces.Count == 0) {
                return (new ProvinceInfo[0], 0);
            }

            List<(ProvinceInfo[], int)> paths = new List<(ProvinceInfo[], int)>();

            foreach (var item in provinces.OrderBy(t => t.Item1)) {
                List<ProvinceInfo> pth = new List<ProvinceInfo>(path.Item1) {
                    item.Item2
                };
                var p = GetPath((pth.ToArray(), path.Item2 + item.Item1), end, hops - 1);
                if (p.Item1?.LastOrDefault() == end) {
                    //paths.Add((p.Item1, p.Item2));
                    return (p.Item1, p.Item2);
                }
            }

            return paths.OrderBy(t => t.Item2).FirstOrDefault();
        }

        public bool IsMoveable(ProvinceInfo province) {
            return province.isLand
                && province.isInteractive
                && (army.BlackStatus
                || province.Country == country
                || country.militaryAccesses.ContainsKey(province.Country))
                || country.IsInWarAgainstCountry(province.Country);
        }
    }
}
