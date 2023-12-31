using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace MonoGame.Training.Components
{
    // Used with collisions and detection
    public class MeshComponent : Component
    {
        public List<Vector2> Vertices { get; set; }
        public List<Tuple<int, int>> Edges { get; set; }
    }
}