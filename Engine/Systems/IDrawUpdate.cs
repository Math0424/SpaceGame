using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems
{
    internal interface IDrawUpdate
    {
        public void Draw(GameTime deltaTime);
    }
}
