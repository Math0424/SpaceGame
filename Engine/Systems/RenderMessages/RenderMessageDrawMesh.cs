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
    internal class RenderMessageDrawMesh : RenderMessageSorting
    {
        public float Transparency { get; private set; }
        public Color Color { get; private set; }
        public ModelInfo Model { get; private set; }
        public RenderMessageDrawMesh(ModelInfo model, float transparency, Color color, Matrix transformMatrix) : base(RenderMessageType.DrawMesh)
        {
            this.Transparency = transparency;
            this.Color = color;
            this.Model = model;
            this.Matrix = transformMatrix;
        }
    }
}
