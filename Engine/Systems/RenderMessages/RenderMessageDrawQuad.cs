using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    internal class RenderMessageDrawQuad : RenderMessage
    {
        public string Texture { get; private set; }
        public Matrix Matrix { get; private set; }
        public bool DrawBack { get; private set; }
        public RenderMessageDrawQuad(string texture, bool drawBack, Matrix transformMatrix) : base(RenderMessageType.DrawQuad)
        {
            this.Texture = texture;
            this.Matrix = transformMatrix;
            this.DrawBack = drawBack;
        }

    }
}
