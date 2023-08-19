using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace YouAskedForIt
{
    public struct CVelocity : IComponentData, IModComponent
    {
        public Vector3 Velocity;

        public CVelocity(Vector3 velocity)
        {
            Velocity = velocity;
        }

        public static implicit operator CVelocity(Vector3 velocity)
        {
            return new CVelocity(velocity);
        }

        public static implicit operator Vector3(CVelocity comp)
        {
            return comp.Velocity;
        }
    }

    public class UpdateMovement : GameSystemBase, IModSystem
    {
        EntityQuery Velocities;
        protected override void Initialise()
        {
            base.Initialise();
            Velocities = GetEntityQuery(typeof(CPosition), typeof(CVelocity));
        }

        protected override void OnUpdate()
        {
            float dt = Time.DeltaTime;

            using NativeArray<Entity> entities = Velocities.ToEntityArray(Allocator.Temp);
            using NativeArray<CPosition> positions = Velocities.ToComponentDataArray<CPosition>(Allocator.Temp);
            using NativeArray<CVelocity> velocities = Velocities.ToComponentDataArray<CVelocity>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                CPosition position = positions[i];
                CVelocity velocity = velocities[i];

                position += velocity.Velocity * dt;
                Set(entity, position);
            }
        }
    }
}
