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
        None = 0,

        LoadMesh = 1 << 0 | Load,
        LoadFont = 1 << 1 | Load,
        LoadEffect = 1 << 2 | Load,
        LoadTexture = 1 << 3 | Load,

        DrawSprite = 1 << 4 | Depth,
        DrawColoredSprite = 1 << 5 | Depth,
        DrawText = 1 << 6 | Depth,

        Sorting = 1 << 7,

        DrawMesh = 1 << 8 | Sorting,
        DrawQuad = 1 << 10 | Sorting,

        DrawLine = 1 << 9,
        DrawBox = 1 << 11,
        DrawSphere = 1 << 12,
        DrawPlane = 1 << 13,

        Load = 1 << 14,
        Depth = 1 << 15,

        CreateRT = 1 << 16 | Load,
        DisposeRT = 1 << 17 | Load,
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
