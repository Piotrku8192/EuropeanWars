using EuropeanWars.Core.Province;

namespace EuropeanWars.Core.Pathfinding {
    public interface IPathfinder {
        ProvinceInfo[] FindPath(ProvinceInfo province);
        bool IsMovable(ProvinceInfo province);
    }
}
