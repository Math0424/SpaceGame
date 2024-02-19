using BulletSharp;
using Microsoft.Xna.Framework;
using Project1.Engine;
using Project1.Engine.Components;
using Project1.Engine.Systems;
using Project1.Engine.Systems.GUI;
using Project2.Engine;
using Project2.Engine.Components;
using Project2.MyGame.EngineComponents;
using Project2.MyGame.GUIElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.MyGame
{
    internal class MainMenuXNAComponent : GameComponent
    {
        public Action<byte, byte, ushort> StartGame;

        private Game _game;
        private World _world;

        public MainMenuXNAComponent(Game game) : base(game)
        {
            _game = game;
            _world = new World(game)
                .AddSystem<Camera>()
                .AddSystem<RenderingSystem>()
                .AddSystem<HudSystem>();
            game.Components.Add(_world);
        }

        public override void Initialize()
        {
            var hud = _world.GetSystem<HudSystem>();
            var gui = new MainMenuGUI(Render.ScreenBounds);
            hud.RegisterElement(gui);
            gui.StartGame += StartGame;
        }

        public override void Update(GameTime gameTime)
        {
            if (_world.GetSystem<Camera>() == null)
                return;
            var camera = _world.GetSystem<Camera>();
            float val = (float)gameTime.TotalGameTime.TotalSeconds / 100;
            Matrix rot = Matrix.CreateFromYawPitchRoll(0.001f, (float)Math.Sin(val) / 250, 0);
            camera.SetWorldMatrix(Matrix.Multiply(camera.WorldMatrix, rot));
        }

        public int DrawOrder => 0;
        public bool Visible => true;

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        protected override void Dispose(bool disposing)
        {
            _game.Components.Remove(_world);
            _world.Dispose();
            base.Dispose(disposing);
        }

    }
}
