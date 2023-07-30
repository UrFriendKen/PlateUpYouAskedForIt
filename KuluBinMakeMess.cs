using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace YouAskedForIt
{
    public struct CKuluBin : IApplianceProperty, IAttachableProperty, IComponentData, IModComponent
    {
        public int CurrentAmount;
    }

    public class KuluBinMakeMess : GenericSystemBase, IModSystem
    {
        EntityQuery Bins;

        protected override void Initialise()
        {
            base.Initialise();
            Bins = GetEntityQuery(typeof(CKuluBin), typeof(CApplianceBin));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> entities = Bins.ToEntityArray(Allocator.Temp);
            using NativeArray<CKuluBin> kuluBins = Bins.ToComponentDataArray<CKuluBin>(Allocator.Temp);
            using NativeArray<CApplianceBin> bins = Bins.ToComponentDataArray<CApplianceBin>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                CKuluBin kuluBin = kuluBins[i];
                CApplianceBin bin = bins[i];

                if (kuluBin.CurrentAmount == bin.CurrentAmount)
                    continue;
                if (bin.CurrentAmount > kuluBin.CurrentAmount)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        Entity messRequestEntity = EntityManager.CreateEntity();
                        Set(messRequestEntity, new CPosition()
                        {
                            Position = new Vector3(Random.Range(Bounds.min.x, Bounds.max.x), 0f, Random.Range(Bounds.min.z, Bounds.max.z)).Rounded()
                        });
                        Set(messRequestEntity, new CMessRequest()
                        {
                            ID = AssetReference.CustomerMess
                        });
                    }
                    CSoundEvent.Create(EntityManager, SoundEvent.MessCreated);
                }
                kuluBin.CurrentAmount = bin.CurrentAmount;
                Set(entity, kuluBin);
            }
        }
    }
}
