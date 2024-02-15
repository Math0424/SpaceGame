using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    internal abstract class RenderMessageSprite : RenderMessage
    {
        public string RenderTarget { get; private set; }
        public float Depth { get; private set; }
        protected RenderMessageSprite(float depth, string RenderTarget, RenderMessageType type) : base(type) 
        {
            this.RenderTarget = RenderTarget ?? "default";
            this.Depth = depth;
        }
    }
}
