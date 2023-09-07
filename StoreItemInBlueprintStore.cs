using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Entities;

namespace YouAskedForIt
{
    public class StoreItemInBlueprintStore : ItemInteractionSystem, IModSystem
    {
        protected override InteractionType RequiredType => InteractionType.Grab;

        private CItemHolder Holder;

        private CItem Item;

        private CBlueprintStore Store;

        protected override bool IsPossible(ref InteractionData data)
        {
            if (!Require<CItemHolder>(data.Interactor, out Holder))
            {
                return false;
            }
            if (!Require<CItem>((Entity)Holder, out Item))
            {
                return false;
            }
            if (!GameData.Main.TryGet(Item.ID, out Item item))
            {
                return false;
            }
            if (!item.ItemStorageFlags.HasFlag(ItemStorage.StackableFood))
            {
                return false;
            }
            if (!Require<CBlueprintStore>(data.Target, out Store))
            {
                return false;
            }
            if (Store.InUse)
            {
                return false;
            }
            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            data.Context.Destroy(Holder.HeldItem);
            data.Context.Set(data.Interactor, default(CItemHolder));
            Store.InUse = true;
            Store.Price = 0;
            Store.ApplianceID = Item.ID;
            Store.BlueprintID = Item.ID;
            Store.HasBeenCopied = false;
            Store.HasBeenMadeFree = false;
            SetComponent(data.Target, Store);
        }
    }
}
