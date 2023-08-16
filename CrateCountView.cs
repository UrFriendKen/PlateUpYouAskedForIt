using Kitchen;
using KitchenData;
using KitchenMods;
using MessagePack;
using System.Runtime.InteropServices;
using TMPro;
using Unity.Collections;
using Unity.Entities;

namespace YouAskedForIt
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct CCrateCountMarker : IApplianceProperty, IAttachableProperty, IComponentData, IModComponent { }

    public class CrateCountView : UpdatableObjectView<CrateCountView.ViewData>
    {
        public class UpdateView : IncrementalViewSystemBase<ViewData>, IModSystem
        {
            EntityQuery Views;
            EntityQuery Crates;

            protected override void Initialise()
            {
                base.Initialise();
                Views = GetEntityQuery(typeof(CCrateCountMarker), typeof(CLinkedView));
                Crates = GetEntityQuery(typeof(CCrateAppliance));
            }

            protected override void OnUpdate()
            {
                if (Views.IsEmpty)
                    return;

                int count = Crates.CalculateEntityCount();

                using NativeArray<CLinkedView> views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                for (int i = 0; i < views.Length; i++)
                {
                    CLinkedView view = views[i];
                    SendUpdate(view, new ViewData()
                    {
                        Count = count
                    });
                }
            }
        }

        [MessagePackObject(false)]
        public class ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public int Count;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<CrateCountView>();

            public bool IsChangedFrom(ViewData check)
            {
                return Count != check.Count;
            }
        }

        public TextMeshPro Text;

        protected override void UpdateData(ViewData data)
        {
            if (Text != null)
            {
                string text = string.Empty;
                if (data.Count > 0)
                    text = $"{data.Count} Crate{(data.Count != 1 ? "s" : string.Empty)}";

                Text.text = text;
            }
        }
    }
}
