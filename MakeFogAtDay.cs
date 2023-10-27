using Kitchen;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace YouAskedForIt
{
    public struct CFog : IComponentData, IModComponent
    {
        public bool Enabled;

        public float SameRoomRevealDist;

        public float AlwaysRevealDist;
    }


    [InternalBufferCapacity(4)]
    public struct CFogInRangePlayer : IBufferElementData
    {
        public int InputSource;
    }

    public class MakeFogAtDay : StartOfDaySystem, IModSystem
    {
        protected override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnUpdate()
        {
            float sameRoomRadius = Main.PrefManager.Get<float>(Main.FOG_OF_WAR_SAME_ROOM_RADIUS_ID);
            float otherRoomRadius = Main.PrefManager.Get<float>(Main.FOG_OF_WAR_OTHER_ROOM_RADIUS_ID);

            DynamicBuffer<CLayoutRoomTile> tiles = Tiles;
            for (int i = 0; i < tiles.Length; i++)
            {
                CLayoutRoomTile tile = tiles[i];
                Entity entity = EntityManager.CreateEntity(typeof(CFog), typeof(CPosition), typeof(CFogInRangePlayer), typeof(CRequiresView));
                Set(entity, new CFog()
                {
                    SameRoomRevealDist = sameRoomRadius,
                    AlwaysRevealDist = Mathf.Min(sameRoomRadius, otherRoomRadius)
                });
                Set(entity, new CPosition(tile.Position));
                Set(entity, default(CDoNotPersist));
                Set(entity, new CRequiresView()
                {
                    Type = Main.FogViewType,
                    ViewMode = ViewMode.World
                });
            }
        }
    }
}
