using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    enum RenderMessageType
    {
        None,

        LoadMesh,
        LoadFont,
        LoadEffect,
        LoadTexture,

        DrawSprite,
        DrawColoredSprite,
        DrawText,

        DrawMesh,
        DrawBasicMesh,

        DrawLine,
        DrawQuad,

        DrawBox,
        DrawSphere,
        DrawPlane,
    }

    internal abstract class RenderMessage
    {
        public RenderMessageType Type { get; private set; }
        public RenderMessage(RenderMessageType type)
        {
            this.Type = type;
        }
    }
}
