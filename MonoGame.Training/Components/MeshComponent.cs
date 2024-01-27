using Microsoft.Xna.Framework;
using MonoGame.Training.Models.Geometry;
using System;
using System.Collections.Generic;

namespace MonoGame.Training.Components
{
    // Used with collisions and detection
    public class MeshComponent : Component
    {
        public List<Vector2> Vertices { get; set; }
        public List<Edge> Edges { get; set; }
        public List<Triangle> Triangles { get; set; }   
    }
}