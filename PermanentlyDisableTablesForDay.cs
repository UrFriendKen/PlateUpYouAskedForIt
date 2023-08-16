using Kitchen;
using KitchenData;
using KitchenMods;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Entities;

namespace YouAskedForIt
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct CExplodedTable : IComponentData, IModComponent { }

    public class PermanentlyDisableTablesForDay : DaySystem, IModSystem
    {
        EntityQuery Groups;

        protected override void Initialise()
        {
            base.Initialise();
            Groups = GetEntityQuery(new QueryHelper()
                .All(
                    typeof(CPatience),
                    typeof(CCustomerSettings),
                    typeof(CGroupMember),
                    typeof(CGroupAwaitingOrder),
                    typeof(CWaitingForItem),
                    typeof(CAssignedTable)));
        }

        protected override void OnUpdate()
        {
            if (!Main.PrefManager.Get<bool>(Main.WRONG_DELIVERY_EXPLOSION_ID) || GetOrDefault<SGameTime>().IsPaused)
                return;

            using NativeArray<Entity> entities = Groups.ToEntityArray(Allocator.Temp);
            using NativeArray<CPatience> groupPatience = Groups.ToComponentDataArray<CPatience>(Allocator.Temp);
            using NativeArray<CCustomerSettings> groupSettings = Groups.ToComponentDataArray<CCustomerSettings>(Allocator.Temp);
            using NativeArray<CAssignedTable> tables = Groups.ToComponentDataArray<CAssignedTable>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                CPatience patience = groupPatience[i];
                CCustomerSettings settings = groupSettings[i];
                CAssignedTable assignedTable = tables[i];
                if (!RequireBuffer(entity, out DynamicBuffer<CWaitingForItem> waitingForItemsBuffer) || !RequireBuffer(assignedTable, out DynamicBuffer<CTableSetGrabPoints> grabPoints))
                    continue;

                for (int j = 0; j < grabPoints.Length; j++)
                {
                    CTableSetGrabPoints grabPoint = grabPoints[j];
                    if (!Has<CApplianceTable>(grabPoint) ||
                        !Require(grabPoint, out CItemHolder holder) ||
                        holder.HeldItem == default ||
                        !Require(holder.HeldItem, out CItem candidate) ||
                        !GameData.Main.TryGet(candidate.ID, out Item candidateGDO) ||
                        !IsItemServable(candidate.ID))
                        continue;

                    bool satisfies = false;
                    int sideID = 0;
                    Item sideGDO = null;
                    for (int k = 0; k < waitingForItemsBuffer.Length; k++)
                    {
                        if (waitingForItemsBuffer[k].Satisfied ||
                            !Require(waitingForItemsBuffer[k].Item, out CItem order) ||
                            !GameData.Main.TryGet(order.ID, out Item orderGDO))
                            continue;

                        if (ItemSatisfiesOrder(candidate, candidateGDO, order, orderGDO, out int alsoProvidedSideID))
                        {
                            satisfies = true;
                            break;
                        }
                        if (sideID == 0 && alsoProvidedSideID != 0)
                        {
                            sideID = alsoProvidedSideID;
                            GameData.Main.TryGet(sideID, out sideGDO, warn_if_fail: true);
                        }
                        if (sideID != 0 && sideGDO != null)
                        {
                            if (ItemSatisfiesOrder(sideID, sideGDO, order, orderGDO, out int _))
                            {
                                satisfies = true;
                                break;
                            }
                        }
                    }
                    if (satisfies)
                    {
                        continue;
                    }

                    Set<CGroupStartLeaving>(entity);
                    Set<CGroupStateChanged>(entity);
                    settings.AddPatience(ref patience, settings.Patience.ItemDeliverBonus);
                    Set(entity, patience);
                    Set<CExplodedTable>(assignedTable);
                    Set<CIsBroken>(assignedTable);
                    break;
                }
            }
        }

        private bool ItemSatisfiesOrder(CItem candidate, Item candidateGDO, CItem order, Item orderGDO, out int sideID)
        {
            sideID = 0;
            if (orderGDO.SatisfiedBy.Count == 0)
            {
                if (candidate.ID == order.ID && (!(candidateGDO is ItemGroup) || !(orderGDO is ItemGroup)))
                    return true;

                int candidateMatchingComponentCount = 0;
                foreach (int candidateComponent in candidate.Items)
                {
                    bool matchingComponentFound = false;
                    for (int i = 0; i < order.Items.Count; i++)
                    {
                        int orderComponent = order.Items[i];
                        if (orderComponent == candidateComponent)
                        {
                            order.Items[i] = 0;
                            matchingComponentFound = true;
                            candidateMatchingComponentCount++;
                            break;
                        }
                    }
                    if (!matchingComponentFound && GameData.Main.TryGet(candidateComponent, out Item componentGDO))
                    {
                        if (!componentGDO.IsMergeableSide)
                        {
                            return false;
                        }
                        sideID = candidateComponent;
                    }
                }
                return candidateMatchingComponentCount == order.Items.Count;
            }
            return candidate.Satisfies(orderGDO);
        }

        private bool IsItemServable(int itemID)
        {
            IEnumerable<Item> items = GameData.Main.Get<Dish>().SelectMany(x => x.UnlocksMenuItems).Select(x => x.Item);
            if (items.Select(x => x.ID).Distinct().Contains(itemID))
                return true;
            if (items.SelectMany(x => x.SatisfiedBy).Select(x => x.ID).Distinct().Contains(itemID))
                return true;
            if (items.Select(x => x.AlwaysOrderAdditionalItem).Where(x => x != 0).Distinct().Contains(itemID))
                return true;
            return false;
        }
    }

    public class UnbreakTablesAtNight : NightSystem, IModSystem
    {
        EntityQuery BrokenTables;

        protected override void Initialise()
        {
            base.Initialise();
            BrokenTables = GetEntityQuery(new QueryHelper()
                .All(typeof(CExplodedTable), typeof(CIsBroken)));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> entities = BrokenTables.ToEntityArray(Allocator.Temp);
            EntityManager.RemoveComponent<CExplodedTable>(entities);
            EntityManager.RemoveComponent<CIsBroken>(entities);
        }
    }
}
