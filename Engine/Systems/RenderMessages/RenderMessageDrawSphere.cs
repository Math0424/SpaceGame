using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    internal class RenderMessageDrawSphere : RenderMessage
    {
        public Vector3 Pos { get; private set; }
        public float Radius { get; private set; }
        public RenderMessageDrawSphere(Vector3 worldPos, float radius) : base (RenderMessageType.DrawSphere)
        {
            Pos = worldPos;
            Radius = radius;
        }
    }
}
