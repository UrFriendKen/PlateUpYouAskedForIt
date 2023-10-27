using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace YouAskedForIt
{
    public class ClearFogAtNight : StartOfNightSystem, IModSystem
    {
        EntityQuery Fog;

        protected override void Initialise()
        {
            base.Initialise();
            Fog = GetEntityQuery(typeof(CFog), typeof(CPosition));
        }

        protected override void OnUpdate()
        {
            EntityManager.DestroyEntity(Fog);
        }
    }
}
