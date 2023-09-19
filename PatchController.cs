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

        internal static bool StaticRequire<T>(Entity e, out T comp) where T : struct, IComponentData
        {
            comp = default;
            return _instance?.Require(e, out comp) ?? false;
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

        internal static bool StaticHasBuffer<T>(Entity e) where T : struct, IBufferElementData
        {
            return _instance?.HasBuffer<T>(e) ?? false;
        }

        internal static bool StaticRequireBuffer<T>(Entity e, out DynamicBuffer<T> buffer) where T : struct, IBufferElementData
        {
            buffer = default;
            return _instance?.RequireBuffer<T>(e, out buffer) ?? false;
        }

        internal static void StaticSet<T>(Entity e, T comp) where  T : struct, IComponentData
        {
            _instance?.Set(e, comp);
        }

        internal static void StaticSet<T>(Entity e) where T : struct, IComponentData
        {
            _instance?.Set<T>(e);
        }
    }
}
