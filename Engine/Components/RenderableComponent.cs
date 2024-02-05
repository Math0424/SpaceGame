using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Engine.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Components
{
    internal abstract class RenderableComponent : EntityComponent
    {
        public bool Visible = true;
        public abstract bool IsVisible(ref Camera cam);
        public abstract void Draw(RenderingSystem system, ref Camera cam);
    }
}
