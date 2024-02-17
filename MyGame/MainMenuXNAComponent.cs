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
                .AddSystem<PhysicsSystem>()
                .AddSystem<HudSystem>();
            game.Components.Add(_world);
        }

        public override void Initialize()
        {
            var hud = _world.GetSystem<HudSystem>();

            var spaceship = _world.CreateEntity()
                .AddComponent(new PositionComponent(Matrix.Identity))//, Matrix.CreateScale(0.01f)))
                .AddComponent(new PrimitivePhysicsComponent(RigidBodyType.Sphere, 10))
                .AddComponent(new SpaceshipController(Matrix.CreateTranslation(new Vector3(0, 0.4f, 0.7f))))
                .AddComponent(new MeshComponent("Models/Cockpit", "Textures/Spaceship/CT", "Textures/Spaceship/ADD"))
                .AddComponent(new MeshRenderingComponent())
                .AddComponent(new MeshAnimationComponent());

            hud.RegisterElement(new HudSpeed(spaceship, new Vector2I(256, 256), "info"));
            hud.RegisterElement(new HudInfo(spaceship, new Vector2I(256, 256), "dashboard"));
            hud.RegisterElement(new HudHealth(spaceship, new Vector2I(256, 256), "health"));

            spaceship.GetComponent<MeshAnimationComponent>().LoadAnimation("openCockpit", "Animations/OpenCockpit.txt");
            _world.GetSystem<WorldGenerationSystem>().CreateRandomWorld(1);
        }

        public override void Update(GameTime gameTime)
        {

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
