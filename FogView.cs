using Controllers;
using Kitchen;
using KitchenMods;
using MessagePack;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace YouAskedForIt
{
    public class FogView : UpdatableObjectView<FogView.ViewData>
    {
        public class UpdateView : IncrementalViewSystemBase<ViewData>, IModSystem
        {
            EntityQuery Views;

            protected override void Initialise()
            {
                base.Initialise();
                Views = GetEntityQuery(typeof(CFog), typeof(CFogInRangePlayer), typeof(CLinkedView));
            }

            protected override void OnUpdate()
            {
                using NativeArray<Entity> entities = Views.ToEntityArray(Allocator.Temp);
                using NativeArray<CFog> fogs = Views.ToComponentDataArray<CFog>(Allocator.Temp);
                using NativeArray<CLinkedView> views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);

                for (int i = 0; i < views.Length; i++)
                {
                    Entity entity = entities[i];
                    CFog fog = fogs[i];
                    DynamicBuffer<CFogInRangePlayer> fogPlayers = GetBuffer<CFogInRangePlayer>(entity);
                    List<int> inputSources = new List<int>();
                    for (int j = 0; j < fogPlayers.Length; j++)
                    {
                        inputSources.Add(fogPlayers[j].InputSource);
                    }
                    SendUpdate(views[i], new ViewData()
                    {
                        Enabled = fog.Enabled,
                        InputSourcesInRange = inputSources
                    });
                }
            }
        }

        [MessagePackObject(false)]
        public class ViewData : IViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public bool Enabled;
            [Key(1)] public List<int> InputSourcesInRange;

            public bool IsChangedFrom(ViewData check)
            {
                if (Enabled != check.Enabled || InputSourcesInRange.Count != check.InputSourcesInRange.Count)
                    return true;
                foreach (int inputSource in InputSourcesInRange)
                {
                    if (!check.InputSourcesInRange.Contains(inputSource))
                        return true;
                }
                foreach (int inputSource in check.InputSourcesInRange)
                {
                    if (!InputSourcesInRange.Contains(inputSource))
                        return true;
                }
                return false;
            }
        }

        public GameObject Fog;

        protected override void UpdateData(ViewData data)
        {
            bool isActive = data.Enabled && !data.InputSourcesInRange.Contains(InputSourceIdentifier.Identifier.Value);
            Fog?.SetActive(isActive);
        }
    }
}
