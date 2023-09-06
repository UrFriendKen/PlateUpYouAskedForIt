using Kitchen;
using KitchenMods;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace YouAskedForIt
{
    public class RandomlyRotateAppliancesAtDay : StartOfDaySystem, IModSystem
    {
        private static readonly HashSet<int> AffectedApplianceIDs = new HashSet<int>()
        {
            -1533430406 // Source - Ice Cream
        };

        EntityQuery Appliances;

        protected override void Initialise()
        {
            base.Initialise();
            Appliances = GetEntityQuery(new QueryHelper()
                .All(typeof(CAppliance), typeof(CPosition))
                .None(typeof(CImmovable), typeof(CFixedRotation), typeof(CMustHaveWall)));
        }

        protected override void OnUpdate()
        {
            if (Has<SPracticeMode>() || !Main.PrefManager.Get<bool>(Main.RANDOMLY_ROTATE_ICE_CREAM_ID))
                return;

            using NativeArray<Entity> entities = Appliances.ToEntityArray(Allocator.Temp);
            using NativeArray<CAppliance> appliances = Appliances.ToComponentDataArray<CAppliance>(Allocator.Temp);
            using NativeArray<CPosition> positions = Appliances.ToComponentDataArray<CPosition>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                CAppliance appliance = appliances[i];
                CPosition position = positions[i];

                if (AffectedApplianceIDs.Contains(appliance.ID))
                {
                    position.Rotation = OrientationHelpers.All.Random().ToRotation();
                    Set(entity, position);
                }
            }
        }
    }
}
