using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project1.Engine.Systems.RenderMessages;
using Project2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.GUI
{
    internal class HudSystem : SystemComponent, IDrawUpdate
    {
        private HudInput _hudInput;

        public HudInput HudInput => _hudInput;
        public Vector2I ScreenCenter { get; private set; }
        public Vector2I ScreenBounds { get; private set; }

        private List<HudNode> _roots = new List<HudNode>();

        public HudSystem()
        {
            if (!Render.IsReady)
                Render.GraphicsReady += GraphicInit;
            else
                GraphicInit();
        }

        private void GraphicInit()
        {
            ScreenCenter = new Vector2I(Render.ScreenBounds.X, Render.ScreenBounds.Y) / 2;
            Render.EnqueueMessage(new RenderMessageLoadTexture("Textures/GUI/ColorableSprite"));
        }

        public void RegisterElement(HudNode root)
        {
            _roots.Add(root);
        }

        public void UnRegisterElement(HudNode root)
        {
            _roots.Remove(root);
        }

        public void Draw(GameTime delta)
        {
            foreach (var x in _roots)
                x.PreDraw((float)delta.ElapsedGameTime.TotalSeconds);
        }

        public override void Update(GameTime delta)
        {
            foreach (var x in _roots)
            {
                _hudInput = new HudInput()
                {
                    Captured = false,
                    Location = (Vector2I)Input.MousePosition(),
                };
                x.PreHandleInput(ref _hudInput);
                x.PreLayout(false);
                x.PreDraw((float)delta.ElapsedGameTime.TotalSeconds);
            }
        }

    }
}
