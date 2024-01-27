namespace MonoGame.Training.Models.Geometry
{
    public class Triangle
    {
        public int Vertex1 { get; }
        public int Vertex2 { get; }
        public int Vertex3 { get; }

        public Triangle(int vertex1, int vertex2, int vertex3)
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
            Vertex3 = vertex3;
        }
    }
}
