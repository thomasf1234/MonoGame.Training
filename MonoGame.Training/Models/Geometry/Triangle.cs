using Microsoft.Xna.Framework;

namespace MonoGame.Training.Models.Geometry
{
    public class Triangle
    {
        public Vector2 Vertex1 { get; }
        public Vector2 Vertex2 { get; }
        public Vector2 Vertex3 { get; }

        public Triangle(Vector2 vertex1, Vector2 vertex2, Vector2 vertex3)
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
            Vertex3 = vertex3;
        }
    }
}
