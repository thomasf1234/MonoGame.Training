using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoGame.Training.Models.Geometry
{
    public class Polygon
    {
        public List<Vector2> Vertices { get; set; }
        public List<Edge> Edges { get; set; }
        public List<Triangle> Triangles { get; set; }
    }
}
