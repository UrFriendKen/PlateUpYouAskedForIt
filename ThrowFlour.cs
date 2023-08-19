using Kitchen;
using KitchenMods;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace YouAskedForIt
{
    [UpdateBefore(typeof(MakePing))]
    public class ThrowFlour : ItemInteractionSystem, IModSystem
    {
        protected override InteractionType RequiredType => InteractionType.Notify;

        CPosition Position;
        Entity HeldItem;

        protected override bool IsPossible(ref InteractionData data)
        {
            if (!Require(data.Interactor, out Position))
                return false;

            if (!Require(data.Interactor, out CItemHolder holder) || holder.HeldItem == default ||
                !Require(holder.HeldItem, out CItem item) || item.ID != 1378842682)   // Flour
                return false;
            HeldItem = holder.HeldItem;
            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            Entity entity = EntityManager.CreateEntity(typeof(CPosition), typeof(CVelocity), typeof(CLifetime), typeof(CRequiresView));
            Set(entity, new CLifetime()
            {
                RemainingLife = 2.3f
            });
            Set(entity, Position);
            Set(entity, new CVelocity()
            {
                Velocity = math.mul(Position.Rotation, Vector3.forward) * 10f
            });
            Set(entity, new CRequiresView()
            {
                Type = Main.FlourEmitterViewType,
                ViewMode = ViewMode.World
            });
            if (HeldItem != default)
                EntityManager.DestroyEntity(HeldItem);
            Set(entity, default(CItemHolder));
        }
    }
}
