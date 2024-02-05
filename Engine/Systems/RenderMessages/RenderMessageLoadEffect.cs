using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    internal class RenderMessageLoadEffect : RenderMessage
    {
        public string Effect { get; private set; }
        public RenderMessageLoadEffect(string effect) : base(RenderMessageType.LoadEffect)
        {
            this.Effect = effect;
        }
    }
}
