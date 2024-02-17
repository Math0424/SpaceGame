using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Project1.Engine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project1.Engine.Systems;
using Project1.Engine.Systems.RenderMessages;
using BulletSharp;
using Project2.Engine;

namespace Project1.Engine.Components
{
    internal enum RigidBodyType
    {
        Sphere,
        Capsule,
        Cylinder,
        Box,
        Plane,
    }

    internal class PrimitivePhysicsComponent : EntityComponent
    {
        public bool IsSleeping => !_rigidBody.IsActive;
        public int EntityId => _entity.Id;

        public Vector3 LinearVelocity => _rigidBody.LinearVelocity.ToXNA();
        public Vector3 AngularVelocity => _rigidBody.AngularVelocity.ToXNA();

        private float _userRadius;
        private float _userMass;

        private CollisionFlags _physicsFlags;
        private CollisionFilterGroups _groupFlags;
        private CollisionFilterGroups _maskFlags;
        private RigidBodyType _type;

        private RigidBody _rigidBody;

        public PrimitivePhysicsComponent(RigidBodyType type, CollisionFlags flags = CollisionFlags.StaticObject, CollisionFilterGroups group = CollisionFilterGroups.StaticFilter, CollisionFilterGroups mask = ~CollisionFilterGroups.StaticFilter)
        {
            _physicsFlags = flags;
            _maskFlags = mask;
            _groupFlags = group;
            _type = type;
            _userRadius = -1;
            _userMass = 0;
        }

        public PrimitivePhysicsComponent(RigidBodyType type, float mass, float radius = -1, CollisionFlags layer = CollisionFlags.None, CollisionFilterGroups group = CollisionFilterGroups.DefaultFilter, CollisionFilterGroups mask = CollisionFilterGroups.AllFilter)
        {
            _maskFlags = mask;
            _groupFlags = group;
            _physicsFlags = layer;
            _type = type;
            _userMass = mass;
            _userRadius = radius;
        }

        public override void Close()
        {
            _entity.World.GetSystem<PhysicsSystem>().World.RemoveCollisionObject(_rigidBody);
            _rigidBody.Dispose();
        }

        public override void Initalize()
        {
            var sim = _entity.World.GetSystem<PhysicsSystem>().World;

            Matrix transform = _entity.Position.WorldMatrix;

            float length = transform.Right.Length();
            float width = transform.Forward.Length();
            float height = transform.Up.Length();

            ConvexInternalShape shape = null;
            switch (_type)
            {
                case RigidBodyType.Box:
                    shape = new BoxShape(transform.HalfExtents().ToBullet());
                    break;
                case RigidBodyType.Sphere:
                    shape = new SphereShape(_userRadius == -1 ? length : _userRadius);
                    break;
                case RigidBodyType.Capsule:
                    shape = new CapsuleShape(width, height);
                    break;
                case RigidBodyType.Cylinder:
                    shape = new CylinderShape(transform.HalfExtents().ToBullet());
                    break;
            }

            Quaternion quat;
            Vector3 pos;
            transform.Decompose(out _, out quat, out pos);
            transform = Matrix.CreateFromQuaternion(quat) * Matrix.CreateTranslation(pos);

            //figure out what to do if its kinematic
            if (_physicsFlags == CollisionFlags.StaticObject)
            {
                using (var bodyInfo = new RigidBodyConstructionInfo(0, null, shape)
                {
                    StartWorldTransform = transform.ToBullet(),
                })
                {
                    _rigidBody = new RigidBody(bodyInfo);
                }
            }
            else
            {
                using (var bodyInfo = new RigidBodyConstructionInfo(_userMass, new DefaultMotionState(transform.ToBullet()), shape, shape.CalculateLocalInertia(_userMass))
                {
                    AngularDamping = 0.05,
                    LinearDamping = 0.01,
                    AngularSleepingThreshold = 0.01,
                    LinearSleepingThreshold = 0.01,
                })
                {
                    _rigidBody = new RigidBody(bodyInfo);
                }
            }
            _rigidBody.UserIndex = _entity.Id;
            _rigidBody.CollisionFlags = _physicsFlags;

            sim.AddRigidBody(_rigidBody, _groupFlags, _maskFlags);
        }

        public void SetWorldMatrix(Matrix matrix)
        {
            _rigidBody.WorldTransform = matrix.ToBullet();
        }

        public void UpdateWorldMatrix()
        {
            if (_physicsFlags == CollisionFlags.StaticObject)
                return;
            _entity.Position.SetWorldMatrix(_rigidBody.WorldTransform.ToXNA());
        }

        public Vector3 VelocityAtPoint(Vector3 point)
        {
            return _rigidBody.GetVelocityInLocalPoint(point.ToBullet()).ToXNA();
        }

        public void AddTorque(Vector3 torque)
        {
            _rigidBody.ActivationState = ActivationState.ActiveTag;
            _rigidBody.ApplyTorqueImpulse(torque.ToBullet());
        }

        public void AddForce(Vector3 force)
        {
            _rigidBody.ActivationState = ActivationState.ActiveTag;
            _rigidBody.ApplyCentralImpulse(force.ToBullet());
        }

        public void AddImpulse(Vector3 force, Vector3 relPos)
        {
            _rigidBody.ActivationState = ActivationState.ActiveTag;
            _rigidBody.ApplyImpulse(force.ToBullet(), relPos.ToBullet());
        }

        public void DebugDraw()
        {
            var pos = _entity.Position;

            if (Vector3.Dot(_entity.World.Render.Camera.Forward, _entity.World.Render.Camera.Translation - pos.Position) > .5f ||
                (_entity.World.Render.Camera.Translation - pos.Position).LengthSquared() > 1000 * 1000)
                return;

            DrawingUtils.DrawMatrix(pos.WorldMatrix);

            // BulletSharp.Math.Vector3 bMin;
            // BulletSharp.Math.Vector3 bMax;
            // _rigidBody.GetAabb(out bMin, out bMax);

            // Vector3 size = bMax.ToXNA() - bMin.ToXNA();
            // Matrix aabb = Matrix.CreateTranslation(pos.WorldMatrix.Translation);
            // aabb.Forward = new Vector3(size.X, 0, 0);
            // aabb.Up = new Vector3(0, size.Y, 0);
            // aabb.Right = new Vector3(0, 0, size.Z);

            Matrix boxMatrix = pos.WorldMatrix;
            boxMatrix.Forward /= 2;
            boxMatrix.Right /= 2;
            boxMatrix.Up /= 2;
            Render.EnqueueMessage(new RenderMessageDrawBox(boxMatrix));

            if (_physicsFlags != CollisionFlags.StaticObject)
            {
                DrawingUtils.DrawLine(pos.Position, _rigidBody.AngularVelocity.ToXNA(), Color.Orange);
                DrawingUtils.DrawLine(pos.Position, _rigidBody.LinearVelocity.ToXNA(), Color.Pink);
                DrawingUtils.DrawWorldText(_entity.World.Render.Camera, $"ID: {_entity.Id}\n AngV: {Math.Round(_rigidBody.AngularVelocity.Length, 2)}\nLinV: {Math.Round(_rigidBody.LinearVelocity.Length, 2)}", pos.Position, IsSleeping ? Color.Blue : Color.Orange);
            }
            else
                DrawingUtils.DrawWorldText(_entity.World.Render.Camera, $"ID: {_entity.Id}", pos.Position, Color.Yellow);
        }

        public void Stop()
        {
            _rigidBody.AngularVelocity = BulletSharp.Math.Vector3.Zero;
            _rigidBody.LinearVelocity = BulletSharp.Math.Vector3.Zero;
        }

    }
}
