using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    internal class RenderMessageLoadTexture : RenderMessage
    {
        public string Texture { get; private set; }
        public RenderMessageLoadTexture(string texture) : base(RenderMessageType.LoadTexture)
        {
            this.Texture = texture;
        }
    }
}
