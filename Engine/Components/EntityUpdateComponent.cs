using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Components
{
    internal abstract class EntityUpdateComponent : EntityComponent
    {
        public abstract void Update(GameTime deltaTime);
    }
}
