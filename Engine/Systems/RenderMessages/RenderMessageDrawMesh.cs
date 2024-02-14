using Microsoft.Xna.Framework;
using Project1.Engine.Components;
using Project2.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{

    internal enum RenderType
    {
        Default = 0,
        ColorMetalAdd = 1 << 0,
        OverrideColor = 1 << 1,
        Transparency = 1 << 2,
    }

    internal class RenderMessageDrawMesh : RenderMessageSorting
    {
        public RenderType RenderType { get; private set; }
        public float Transparency { get; private set; }
        public Color ColorOverride { get; private set; }
        public ModelInfo Model { get; private set; }
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
