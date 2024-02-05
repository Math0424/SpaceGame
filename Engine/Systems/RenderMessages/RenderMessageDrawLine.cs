using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    internal class RenderMessageDrawLine : RenderMessage
    {
        public Vector3 Pos1 { get; private set; }
        public Vector3 Pos2 { get; private set; }
        public Color Color { get; private set; }
        public RenderMessageDrawLine(Vector3 pos1, Vector3 pos2, Color color) : base(RenderMessageType.DrawLine)
        {
            this.Pos1 = pos1;
            this.Pos2 = pos2;
            this.Color = color;
        }
    }
}
