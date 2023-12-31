using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoGame.Training.Components
{
    public class MenuComponent : Component
    {
        public List<string> MenuItems { get; set; }
        public int MenuItemSelectedIndex { get; set; } 
    }
}