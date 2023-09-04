using Kitchen;
using KitchenData;
using KitchenMods;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Entities;

namespace YouAskedForIt
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct CDestroyIfOnFireAtNight : IApplianceProperty, IAttachableProperty, ICommandData, IModComponent { }

    public class DestroyAppliancesOnFireOvernight : RestaurantSystem, IModSystem
    {
        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct CWasOnFire : IComponentData, IModComponent { }

        EntityQuery WasOnFire;
        EntityQuery Appliances;
        EntityQuery AppliancesOnFireNight;

        protected override void Initialise()
        {
            base.Initialise();
            WasOnFire = GetEntityQuery(typeof(CWasOnFire));
            Appliances = GetEntityQuery(typeof(CDestroyIfOnFireAtNight));
            AppliancesOnFireNight = GetEntityQuery(typeof(CDestroyIfOnFireAtNight), typeof(CWasOnFire));
        }

        protected override void OnUpdate()
        {
            if (!Main.PrefManager.Get<bool>(Main.DESTROY_PROTECTORS_ON_FIRE_ID))
            {
                if (!WasOnFire.IsEmpty)
                    EntityManager.RemoveComponent<CWasOnFire>(WasOnFire);
                return;
            }

            if (Has<SIsDayTime>())
            {
                using NativeArray<Entity> entities = Appliances.ToEntityArray(Allocator.Temp);
                for (int i = 0; i < entities.Length; i++)
                {
                    Entity entity = entities[i];
                    bool onFire = Has<CIsOnFire>(entity);
                    if (onFire && !Has<CWasOnFire>(entity))
                        Set<CWasOnFire>(entity);
                    else if (!onFire && Has<CWasOnFire>(entity))
                        EntityManager.RemoveComponent<CWasOnFire>(entity);
                }
            }
            else
            {
                EntityManager.DestroyEntity(AppliancesOnFireNight);
            }
        }
    }
}
