using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine
{
    internal abstract class EntityComponent
    {
        protected readonly Entity _entity;
        public virtual void Initalize() { }
        public virtual void Close() {}
    }

    internal abstract class SystemComponent
    {
        public SystemComponent() { }
        public virtual void Update(GameTime delta) { }
        public virtual void Close() {}
    }
}
