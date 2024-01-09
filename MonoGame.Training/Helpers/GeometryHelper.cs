using Microsoft.Xna.Framework;
using MonoGame.Training.Models.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Training.Helpers
{
    public class GeometryHelper
    {
        // Triangulate the polygon using the ear clipping algorithm with guard clause
        public List<Triangle> Triangulate(List<Vector2> vertices)
        {
            // Clone to avoid changing original
            vertices = vertices.ToList();
            /*if (!IsSimplePolygon(vertices, edges))
            {
                throw new ArgumentException("The input polygon is not a simple polygon or it is self-intersecting.");
            }*/

            List<Triangle> triangles = new List<Triangle>();

            if (vertices.Count == 3)
            {
                triangles.Add(new Triangle(vertices[0], vertices[1], vertices[2]));
                return triangles;
            }

            while (vertices.Count >= 3)
            {
                int earTipIndex = FindEarTip(vertices);

                int prevIndex = (earTipIndex + vertices.Count - 1) % vertices.Count;
                int nextIndex = (earTipIndex + 1) % vertices.Count;

                triangles.Add(new Triangle(vertices[prevIndex], vertices[earTipIndex], vertices[nextIndex]));

                vertices.RemoveAt(earTipIndex);
            }

            return triangles;
        }

        // Helper function to find an ear tip in the polygon
        public int FindEarTip(List<Vector2> vertices)
        {
            int count = vertices.Count;

            for (int i = 0; i < count; i++)
            {
                Vector2 prev = vertices[(i + count - 1) % count];
                Vector2 current = vertices[i];
                Vector2 next = vertices[(i + 1) % count];

                if (IsEar(prev, current, next, vertices))
                {
                    return i;
                }
            }

            throw new InvalidOperationException("No ear tip found.");
        }

        // Helper function to check if a vertex is an ear
        public bool IsEar(Vector2 prev, Vector2 current, Vector2 next, List<Vector2> vertices)
        {
            int count = vertices.Count;

            for (int i = 0; i < count; i++)
            {
                if (i != vertices.IndexOf(prev) && i != vertices.IndexOf(current) && i != vertices.IndexOf(next))
                {
                    Vector2 testPoint = vertices[i];

                    if (IsPointInTriangle(prev, current, next, testPoint))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsPointInTriangle(Vector2 vertex1, Vector2 vertex2, Vector2 vertex3, Vector2 point)
        {
            // Calculate the barycentric coordinates
            // of point P with respect to triangle ABC
            double denominator = ((vertex2.Y - vertex3.Y) * (vertex1.X - vertex3.X) + (vertex3.X - vertex2.X) * (vertex1.Y - vertex3.Y));
            double a = ((vertex2.Y - vertex3.Y) * (point.X - vertex3.X) + (vertex3.X - vertex2.X) * (point.Y - vertex3.Y)) / denominator;
            double b = ((vertex3.Y - vertex1.Y) * (point.X - vertex3.X) + (vertex1.X - vertex3.X) * (point.Y - vertex3.Y)) / denominator;
            double c = 1 - a - b;

            // Check if all barycentric coordinates
            // are non-negative
            return a >= 0 && b >= 0 && c >= 0;
        }

        public bool IsConvex(List<Vector2> vertices)
        {
            // For each set of three adjacent points A, B, C,
            // find the cross product AB x BC. If the sign of
            // all the cross products is the same, the angles
            // are all positive or negative (depending on the
            // order in which we visit them) so the polygon
            // is convex.
            bool hasNegative = false;
            bool hasPositive = false;
            int vertexCount = vertices.Count;
            int b, c;
            for (int a = 0; a < vertexCount; a++)
            {
                b = (a + 1) % vertexCount;
                c = (b + 1) % vertexCount;

                Vector2 ab = new Vector2(vertices[b].X - vertices[a].X, vertices[b].Y - vertices[a].Y);
                Vector2 bc = new Vector2(vertices[c].X - vertices[b].X, vertices[c].Y - vertices[b].Y);

                double crossProduct = CrossProduct(ab, bc);

                if (crossProduct < 0)
                {
                    hasNegative = true;
                }
                else if (crossProduct > 0)
                {
                    hasPositive = true;
                }

                if (hasNegative && hasPositive)
                {
                    return false;
                }
            }

            // If we got this far, the polygon is convex.
            return true;
        }

        public double CrossProduct(Vector2 v1, Vector2 v2)
        {
            double z = v1.X * v2.Y - v2.X * v1.Y;
            return z;
        }
    }
}
