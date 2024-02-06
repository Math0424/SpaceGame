using BulletSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1.Engine.Components;
using Project1.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems
{
    // General concepts taken from
    // https://theswissbay.ch/pdf/Gentoomen%20Library/Game%20Development/Programming/Game%20Physics%20Engine%20Development.pdf
    // http://www.r-5.org/files/books/computers/algo-list/realtime-3d/Christer_Ericson-Real-Time_Collision_Detection-EN.pdf
    // http://www.chrishecker.com/images/b/bb/Gdmphys4.pdf

    internal class PhysicsSystem : SystemComponent
    {
        public DiscreteDynamicsWorld World;
        /// <summary>
        /// [ent] collided with [ent] at [pos] with [normal] and [force]
        /// </summary>
        public Action<int, int, Vector3, Vector3, float> Collision;

        private World _world;
        private double _accumulatedTime;
        private const float physicsTimeStep = 0.005f;

        private bool _debugMode;
        private RenderingSystem _render;

        private CollisionConfiguration _collisionConfiguration;
        private CollisionDispatcher _dispatcher;
        private DbvtBroadphase _broadphase;

        public override void Close()
        {
            World.Dispose();
            _collisionConfiguration.Dispose();
            _dispatcher.Dispose();
            _broadphase.Dispose();
        }

        public PhysicsSystem(World world, RenderingSystem render)
        {
            _world = world;
            _debugMode = false;
            _render = render;
            _render.DoDraw += DebugDraw;

            _collisionConfiguration = new DefaultCollisionConfiguration();
            _dispatcher = new CollisionDispatcher(_collisionConfiguration);
            _broadphase = new DbvtBroadphase();
            World = new DiscreteDynamicsWorld(_dispatcher, _broadphase, null, _collisionConfiguration);
            //World.Gravity = new BulletSharp.Math.Vector3(0, -9.8, 0); - no gravity in space
        }

        public void DebugDraw()
        {
            if (_debugMode)
            {
                DrawingUtils.DrawWorldText(_render, $"{World.NumCollisionObjects} physics objects", Vector3.Zero, Color.Thistle);

                var physicsObjects = _world.GetEntityComponents<PrimitivePhysicsComponent>();
                if (physicsObjects == null)
                    return;

                foreach (var physicObject in physicsObjects)
                    physicObject.DebugDraw(_render);
            }
        }

        public override void Update(GameTime delta)
        {
            if (Input.IsNewKeyDown(Keys.F12))
                _debugMode = !_debugMode;

            _accumulatedTime += delta.ElapsedGameTime.TotalSeconds;
            while (_accumulatedTime >= physicsTimeStep)
            {
                World.StepSimulation(physicsTimeStep);
                _accumulatedTime -= physicsTimeStep;
            }

            var physicsObjects = _world.GetEntityComponents<PrimitivePhysicsComponent>();
            if (physicsObjects == null)
                return;
            foreach (var x in physicsObjects)
                if (x.EntityId != -1)
                    x.UpdateWorldMatrix();

            for (int i = 0; i < World.Dispatcher.NumManifolds; i++)
            {
                PersistentManifold contactManifold = World.Dispatcher.GetManifoldByIndexInternal(i);
                CollisionObject obA = contactManifold.Body0;
                CollisionObject obB = contactManifold.Body1;
                
                ManifoldPoint pt = contactManifold.GetContactPoint(0);
                Collision?.Invoke(obA.UserIndex, obB.UserIndex, pt.PositionWorldOnA.ToXNA(), pt.NormalWorldOnB.ToXNA(), (float)pt.AppliedImpulse);
            }

        }
    }
}
