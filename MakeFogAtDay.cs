using Kitchen;
using KitchenMods;
using Unity.Entities;

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
            DynamicBuffer<CLayoutRoomTile> tiles = Tiles;
            for (int i = 0; i < tiles.Length; i++)
            {
                CLayoutRoomTile tile = tiles[i];
                Entity entity = EntityManager.CreateEntity(typeof(CFog), typeof(CPosition), typeof(CFogInRangePlayer), typeof(CRequiresView));
                Set(entity, new CFog()
                {
                    SameRoomRevealDist = 3f,
                    AlwaysRevealDist = 1.5f
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
