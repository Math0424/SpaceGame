using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    internal abstract class RenderMessageDepth : RenderMessage
    {
        public float Depth { get; private set; }
        protected RenderMessageDepth(float depth, RenderMessageType type) : base(type) 
        {
            this.Depth = depth;
        }
    }
}
