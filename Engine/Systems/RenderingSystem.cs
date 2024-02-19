using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1.Engine.Components;
using Project1.Engine.Systems.RenderMessages;
using Project2.Engine;
using Project2.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1.Engine.Systems
{
    internal class RenderingSystem : SystemComponent, IDrawUpdate
    {
        public Camera Camera { get; private set; }
        public Vector2I ScreenBounds { get; private set; }
        public Action DoDraw;
        public Action DoDebugDraw;
        public Action OnGraphicsReady;

        private GameTime tickTime;
        private bool _debugMode;
        private World _world;
        private Game _game;

        public RenderingSystem(World world, Game game, Camera camera)
        {
            _game = game;
            Camera = camera;
            _world = world;
            Render.GraphicsReady += SetupCam;
            if (Render.IsReady)
                SetupCam();
        }

        private void SetupCam()
        {
            Camera.SetupProjection(Render.ScreenBounds.X, Render.ScreenBounds.Y, 90);
        }

        public void Draw(GameTime delta)
        {
            int rendering = 0;
            var drawables = _world.GetEntityComponents<RenderableComponent>();
            if (drawables != null)
            {
                var camera = Camera;
                foreach (var x in drawables)
                {
                    if (x.Visible && x.IsVisible(ref camera))
                    {
                        rendering++;
                        x.Draw(ref camera);
                    }
                }
            }

            Render.Instance.DrawScene(Camera);

            if (_debugMode)
            {
                var frameData = Render.FrameStats;
                long ticksTaken = (frameData.FrameTickTime) / 10000;
                string message = $"Rendering Debug:\n" +
                    $"Time: {Math.Round(delta.TotalGameTime.TotalMilliseconds / 1000, 2)}s\n" +
                    $"FPS: {Math.Round(delta.ElapsedGameTime.TotalSeconds * 1000, 2)}ms {Math.Round((ticksTaken / delta.ElapsedGameTime.TotalMilliseconds) * 100)}%\n" +
                    $"TPS: {Math.Round(tickTime.ElapsedGameTime.TotalSeconds * 1000, 2)}ms\n" +
                    $"Entities: {_world.EntityCount}\n" +
                    $"Drawn: {rendering}/{drawables?.Count() ?? -1}\n" +
                    $"DrawCalls: {frameData.DrawCalls} / {frameData.RenderMessages}\n" +
                    $"Triangles: {frameData.TriangleCount}\n" +
                    $"Textures: {frameData.TextureCount}\n" +
                    $"Assets: {frameData.LoadedAssets}\n" +
                    $"RenderTargets: {frameData.RenderTargets}\n" +
                    $"Pos: [{Math.Round(Camera.Translation.X, 2)}, {Math.Round(Camera.Translation.Y, 2)}, {Math.Round(Camera.Translation.Z, 2)}]";
                Render.EnqueueMessage(new RenderMessageDrawText("Fonts/Debug", message, 1, 1, Vector2.Zero, Color.Yellow));
            }
        } 

        public override void Update(GameTime delta)
        {
            tickTime = delta;
            if (Input.IsNewKeyDown(Keys.F11))
            {
                _debugMode = !_debugMode;
            }
        }

        public override void Close()
        {
            Render.GraphicsReady -= SetupCam;
        }
    }
}
