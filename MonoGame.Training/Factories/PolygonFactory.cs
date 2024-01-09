using Microsoft.Xna.Framework;
using MonoGame.Training.Helpers;
using MonoGame.Training.Models.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Training.Factories
{
    public class PolygonFactory
    {
        private GeometryHelper _geometryHelper;
        public PolygonFactory(GeometryHelper geometryHelper)
        {
            _geometryHelper = geometryHelper;
        }

        public Polygon Create(List<Vector2> vertices)
        {
            if (vertices.Count < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(vertices));
            }

            if (!_geometryHelper.IsConvex(vertices))
            {
                throw new ArgumentException("The vertices do not construct a convex polygon.");
            }

            var edges = new List<Edge>();
            var vertexCount = vertices.Count;
            for (int i = 0; i < vertexCount; ++i)
            {
                var nextVertexIndex = (i + 1) % vertexCount;
                var edge = new Edge(i, nextVertexIndex);

                edges.Add(edge);
            }

            var triangles = _geometryHelper.Triangulate(vertices);

            var polygon = new Polygon()
            {
                Vertices = vertices,
                Edges = edges,
                Triangles = triangles
            };

            return polygon;
        }
    }
}
