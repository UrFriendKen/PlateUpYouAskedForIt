using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace YouAskedForIt
{
    public class PatchController : GenericSystemBase, IModSystem
    {
        static PatchController _instance;
        protected override void Initialise()
        {
            base.Initialise();
            _instance = this;
        }

        protected override void OnUpdate()
        {
        }

        internal static bool StaticHas<T>(Entity e) where T : struct, IComponentData
        {
            return _instance?.Has<T>(e) ?? false;
        }

        internal static bool StaticRemove<T>(Entity e) where T : struct, IComponentData
        {
            if (StaticHas<T>(e) && _instance.EntityManager != null)
            {
                _instance.EntityManager.RemoveComponent<T>(e);
                return true;
            }
            return false;
        }
    }
}
