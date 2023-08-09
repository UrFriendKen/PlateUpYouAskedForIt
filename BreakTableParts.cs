using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace YouAskedForIt
{
    public class BreakTableParts : DaySystem, IModSystem
    {
        EntityQuery BrokenTables;

        protected override void Initialise()
        {
            base.Initialise();
            BrokenTables = GetEntityQuery(new QueryHelper()
                .All(typeof(CTableSet), typeof(CTableSetParts)));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> entities = BrokenTables.ToEntityArray(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];

                bool shouldBreak = Has<CPreventUse>(entity);

                if (!RequireBuffer(entity, out DynamicBuffer<CTableSetParts> tableSetParts))
                    continue;

                bool shouldPlaySound = false;
                for (int j = 0; j < tableSetParts.Length; j++)
                {
                    Entity tableSetPartEntity = tableSetParts[j];
                    if (shouldBreak)
                    {
                        if (!Has<CIsBroken>(tableSetPartEntity))
                        {
                            if (Require(tableSetPartEntity, out CPosition tablePosition))
                            {
                                CreateExplosion(tablePosition);
                                shouldPlaySound = true;
                            }
                            Set<CIsBroken>(tableSetPartEntity);
                        }
                    }
                    else if (Has<CIsBroken>(tableSetPartEntity))
                    {
                        EntityManager.RemoveComponent<CIsBroken>(tableSetPartEntity);
                    }
                }
                if (shouldPlaySound)
                {
                    CreateExplosionSound();
                }
            }
        }

        void CreateExplosion(CPosition position)
        {
            Entity entity = EntityManager.CreateEntity();
            Set(entity, new CRequiresView()
            {
                Type = Main.ExplosionEffectViewType,
                ViewMode = ViewMode.World
            });
            Set(entity, position);
            Set(entity, new CLifetime()
            {
                RemainingLife = 1.5f
            });
        }

        void CreateExplosionSound()
        {
            Entity entity = EntityManager.CreateEntity();
            Set(entity, new CRequiresView()
            {
                Type = Main.ExplosionEffectSoundViewType,
                ViewMode = ViewMode.World
            });
            Set(entity, new CPosition()
            {
                Position = Vector3.zero
            });
            Set(entity, new CLifetime()
            {
                RemainingLife = 1.5f
            });
        }
    }
}
