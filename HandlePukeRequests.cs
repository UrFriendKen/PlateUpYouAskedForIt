using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace YouAskedForIt
{
    public struct CRequestPuke : IComponentData, IModComponent
    {
        public int Count;
    }

    public class HandlePukeRequests : RestaurantSystem, IModSystem
    {
        EntityQuery Groups;

        protected override void Initialise()
        {
            base.Initialise();
            Groups = GetEntityQuery(typeof(CGroupMember), typeof(CRequestPuke));
        }

        protected override void OnUpdate()
        {
            float dt = Time.DeltaTime;

            using NativeArray<Entity> entities = Groups.ToEntityArray(Allocator.Temp);
            using NativeArray<CRequestPuke> requests = Groups.ToComponentDataArray<CRequestPuke>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                CRequestPuke request = requests[i];
                if (!Has<CGroupAwaitingOrder>(entity) ||
                    request.Count <= 0 ||
                    !RequireBuffer(entity, out DynamicBuffer<CGroupMember> groupMembers))
                {
                    EntityManager.RemoveComponent<CRequestPuke>(entity);
                    continue;
                }

                if (Random.value > (2f * dt))
                    continue;

                CGroupMember cGroupMember = groupMembers[Random.Range(0, groupMembers.Length)];
                if (!Require(cGroupMember.Customer, out CPosition customerPosition))
                    continue;

                Entity messRequestEnt = EntityManager.CreateEntity(typeof(CPosition), typeof(CMessRequest));
                Set(messRequestEnt, customerPosition);
                Set(messRequestEnt, new CMessRequest
                {
                    ID = AssetReference.CustomerMess
                });
                CSoundEvent.Create(EntityManager, SoundEvent.MessCreated);

                request.Count--;
                Set(entity, request);
            }
        }
    }
}
