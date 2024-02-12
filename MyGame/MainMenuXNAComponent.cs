using BulletSharp;
using Microsoft.Xna.Framework;
using Project1.Engine;
using Project1.Engine.Components;
using Project1.Engine.Systems;
using Project1.Engine.Systems.GUI;
using Project2.MyGame.EngineComponents;
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
        public Action<string, int> StartGame;

        private Game _game;
        private World _world;

        public MainMenuXNAComponent(Game game) : base(game)
        {
            _game = game;
            _world = new World(game)
                .AddSystem<Camera>()
                .AddSystem<RenderingSystem>()
                .AddSystem<WorldGenerationSystem>()
                .AddSystem<PhysicsSystem>();
                //.AddSystem<HudSystem>();
            game.Components.Add(_world);
        }

        public override void Initialize()
        {
            // var hud = _world.GetSystem<HudSystem>();

            // var menu = new MainMenuGUI(hud.Root);
            // menu.StartGame += StartGame;

            _world.CreateEntity()
                .AddComponent(new PositionComponent(Matrix.Identity, Matrix.CreateScale(0.01f)))
                .AddComponent(new PrimitivePhysicsComponent(RigidBodyType.Sphere, Engine.Components.RigidBodyFlags.Dynamic, 10))
                .AddComponent(new SpaceshipController(Matrix.CreateTranslation(new Vector3(0, 0.4f, 0.7f))))
                .AddComponent(new MeshComponent("Models/Cockpit"));//, "Textures/Shotgun/shotgun_CM", "Textures/Shotgun/shotgun_ADD"));


            _world.GetSystem<WorldGenerationSystem>().CreateRandomWorld(.75f);
        }

        public override void Update(GameTime gameTime)
        {
            _world.GetSystem<WorldGenerationSystem>().DebugDraw();
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
