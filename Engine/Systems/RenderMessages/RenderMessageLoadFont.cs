using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    internal class RenderMessageLoadFont : RenderMessage
    {
        public string Font { get; private set; }
        public RenderMessageLoadFont(string font) : base(RenderMessageType.LoadFont)
        {
            this.Font = font;
        }
    }
}
