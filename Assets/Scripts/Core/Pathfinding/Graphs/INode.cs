using Roy_T.AStar.Primitives;
using System.Collections.Generic;

namespace Roy_T.AStar.Graphs {
    public interface INode {
        Position Position { get; set; }
        IList<IEdge> Incoming { get; }
        IList<IEdge> Outgoing { get; }
        bool Movable { get; set; }
    }
}
