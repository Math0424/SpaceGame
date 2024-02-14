using Microsoft.Xna.Framework;
using Project1.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{

    internal enum RenderType
    {
        Default,
        ColorMetalAdd,
        OverrideColor,
    }

    internal class RenderMessageDrawMesh : RenderMessage
    {
        public RenderType RenderType { get; private set; }
        public float Transparency { get; private set; }
        public Color ColorOverride { get; private set; }
        public ModelInfo Model { get; private set; }
        public Matrix Matrix { get; private set; }
        public RenderMessageDrawMesh(ModelInfo model, RenderType type, float transparency, Color color, Matrix transformMatrix) : base(RenderMessageType.DrawMesh)
        {
            this.Transparency = transparency;
            this.ColorOverride = color;
            this.RenderType = type;
            this.Model = model;
            this.Matrix = transformMatrix;
        }
    }
}
