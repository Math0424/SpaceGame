using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project1.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.GUI
{
    internal class HudSystem : SystemComponent, IDrawUpdate
    {
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        private Game _game;
        private RenderingSystem _render;
        private HudInput _hudInput;

        public HudInput HudInput => _hudInput;
        public HudRoot Root { get; private set; }
        public Vector2I ScreenCenter { get; private set; }
        public Vector2I ScreenBounds { get; private set; }

        public HudSystem(Game game, RenderingSystem render)
        {
            _game = game;
            _render = render;
            if (!_render.GraphicsReady)
                _render.OnGraphicsReady += GraphicInit;
            else
                GraphicInit();
        }

        private void GraphicInit()
        {
            ScreenCenter = new Vector2I((int)_render.ScreenBounds.X / 2, (int)_render.ScreenBounds.Y / 2);
            Root = new HudRoot(this)
            {
                Visible = true,
                InputEnabled = true,
                Position = ScreenCenter,
            };
            _render.EnqueueMessage(new RenderMessageLoadTexture("Textures/GUI/ColorableSprite"));
        }

        public void Draw(GameTime delta)
        {
            if (Root != null)
                Root.PreDraw((float)delta.ElapsedGameTime.TotalSeconds);
        }

        public void DrawSprite(string sprite, Vector2I pos, Vector2I bounds, float depth)
        {
            Rectangle rec = new Rectangle(pos.X - bounds.X / 2, pos.Y - bounds.Y / 2, bounds.X, bounds.Y);
            _render.EnqueueMessage(new RenderMessageDrawSprite(sprite, rec, depth));
        }

        public void DrawColoredSprite(string sprite, Vector2I pos, Vector2I bounds, float depth, Color color)
        {
            Rectangle rec = new Rectangle(pos.X - bounds.X / 2, pos.Y - bounds.Y / 2, bounds.X, bounds.Y);
            _render.EnqueueMessage(new RenderMessageDrawColoredSprite(sprite, rec, depth, color));
        }

        public void DrawText(string font, string text, float scale, float depth, Vector2I pos, Color color, TextDrawOptions options = TextDrawOptions.Default)
        {
            _render.EnqueueMessage(new RenderMessageDrawText(font, text, scale, depth, pos, color, options));
        }

        public override void Update(GameTime delta)
        {
            if (Root != null)
            {
                _hudInput = new HudInput()
                {
                    Captured = false,
                    Location = (Vector2I)Input.MousePosition(),
                };
                Root.PreHandleInput(ref _hudInput);
                Root.PreLayout(false);
            }
        }

    }
}
