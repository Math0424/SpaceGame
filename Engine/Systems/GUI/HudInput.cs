using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.GUI
{
    internal struct HudInput
    {

        public bool Captured;
        public Vector2I Location;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsWithinBounds(Vector2I pos, Vector2I bounds)
        {
            return Location.X >= pos.X - (bounds.X / 2) &&
                Location.Y >= pos.Y - (bounds.Y / 2) &&
                Location.X <= pos.X + (bounds.X / 2) &&
                Location.Y <= pos.Y + (bounds.Y / 2);
        }

    }
}
