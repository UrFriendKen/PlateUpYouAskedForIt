using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Entities;

namespace YouAskedForIt
{
    public class RetrieveItemInBlueprintStore : ItemInteractionSystem, IModSystem
    {
        protected override InteractionType RequiredType => InteractionType.Grab;

        private CItemHolder Holder;

        private CBlueprintStore Store;

        protected override bool IsPossible(ref InteractionData data)
        {
            if (!Require<CItemHolder>(data.Interactor, out Holder))
            {
                return false;
            }
            if (!Require<CBlueprintStore>(data.Target, out Store))
            {
                return false;
            }
            if (!GameData.Main.TryGet(Store.BlueprintID, out Item item))
            {
                return false;
            }
            if (Holder.HeldItem != default(Entity))
            {
                return false;
            }
            if (!Store.InUse)
            {
                return false;
            }
            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            Entity entity = data.Context.CreateItem(Store.BlueprintID);
            data.Context.UpdateHolder(entity, data.Interactor);
            data.Context.Set(data.Interactor, new CItemHolder
            {
                HeldItem = entity
            });
            if (Store.HasBeenCopied)
            {
                Store.HasBeenCopied = false;
            }
            else
            {
                Store.InUse = false;
                Store.ApplianceID = 0;
                Store.Price = 0;
                Store.BlueprintID = 0;
            }
            Store.HasBeenMadeFree = false;
            SetComponent(data.Target, Store);
        }
    }
}
