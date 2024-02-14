using Microsoft.Xna.Framework;
using Project2.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    internal class RenderMessageDrawQuad : RenderMessageSorting
    {
        public string Texture { get; private set; }
        public bool DrawBack { get; private set; }
        public RenderMessageDrawQuad(string texture, bool drawBack, Matrix transformMatrix) : base(RenderMessageType.DrawQuad)
        {
            this.Texture = texture;
            this.Matrix = transformMatrix;
            this.DrawBack = drawBack;
        }

    }
}
