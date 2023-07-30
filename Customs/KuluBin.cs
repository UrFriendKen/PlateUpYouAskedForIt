using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.References;
using KitchenLib.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace YouAskedForIt.Customs
{
    public class KuluBin : CustomAppliance
    {
        public override string UniqueNameID => "kuluBin";

        public override GameObject Prefab => Main.Bundle.LoadAsset<GameObject>("KuluBin");

        public override List<IApplianceProperty> Properties => new List<IApplianceProperty>()
        {
            new CApplianceBin()
            {
                Capacity = 5,
                EmptyBinItem = ItemReferences.BinBag
            },
            new CKuluBin()
        };

        public override IEffectRange EffectRange => new CEffectRangeTiles()
        {
            Tiles = 2,
            PassThroughWalls = false
        };
        public override IEffectType EffectType => new CTableModifier()
        {
            OrderingModifiers = new OrderingValues()
            {
                MessFactor = 1.5f
            }
        };
        public override IEffectCondition EffectCondition => new CEffectAlways();
        public override EffectRepresentation EffectRepresentation => GDOUtils.GetExistingGDO(EffectRepresentationReferences.BinPenalty) as EffectRepresentation;

        public override List<(Locale, ApplianceInfo)> InfoList => new List<(Locale, ApplianceInfo)>()
        {
            (Locale.English, LocalisationUtils.CreateApplianceInfo("Kulu Bin", "That's one grumpy bin...", new List<Appliance.Section>(), new List<string>()))
        };

        public override bool IsPurchasable => false;
        public override bool IsPurchasableAsUpgrade => false;
        public override ShoppingTags ShoppingTags => ShoppingTags.Misc;
        public override PriceTier PriceTier => PriceTier.Cheap;

        static FieldInfo f_Items = typeof(BinView).GetField("Items", BindingFlags.NonPublic | BindingFlags.Instance);
        public override void SetupPrefab(GameObject prefab)
        {
            MaterialUtils.ApplyMaterial(prefab, "Bin (1)/Bin Liner", new Material[] { MaterialUtils.GetExistingMaterial("Bin Bag") });
            MaterialUtils.ApplyMaterial(prefab, "Bin (1)/Cylinder", new Material[] { MaterialUtils.GetExistingMaterial("Plastic - Blue") });
            MaterialUtils.ApplyMaterial(prefab, "Bin (1)/Cylinder/Cube", new Material[] { MaterialUtils.GetExistingMaterial("Plastic - Black Dark") });
            MaterialUtils.ApplyMaterial(prefab, "Bin (1)/Cylinder/Cube.001", new Material[] { MaterialUtils.GetExistingMaterial("Plastic - Black Dark") });
            MaterialUtils.ApplyMaterial(prefab, "Bin (1)/Cylinder/Cube.002", new Material[] { MaterialUtils.GetExistingMaterial("Plastic - Black Dark") });

            MaterialUtils.ApplyMaterial(prefab, "Bin (1)/Contents", new Material[] { MaterialUtils.GetExistingMaterial("Mess") });
            MaterialUtils.ApplyMaterial(prefab, "Bin (1)/Contents.001", new Material[] { MaterialUtils.GetExistingMaterial("Mess") });
            MaterialUtils.ApplyMaterial(prefab, "Bin (1)/Contents.002", new Material[] { MaterialUtils.GetExistingMaterial("Mess") });
            MaterialUtils.ApplyMaterial(prefab, "Bin (1)/Contents.003", new Material[] { MaterialUtils.GetExistingMaterial("Mess") });
            MaterialUtils.ApplyMaterial(prefab, "Bin (1)/Contents.004", new Material[] { MaterialUtils.GetExistingMaterial("Mess") });

            if (prefab.GetComponent<BinView>() == null)
            {
                BinView binView = prefab.AddComponent<BinView>();
                List<GameObject> items = new List<GameObject>()
                {
                    prefab.GetChild("Bin (1)/Contents"),
                    prefab.GetChild("Bin (1)/Contents.001"),
                    prefab.GetChild("Bin (1)/Contents.002"),
                    prefab.GetChild("Bin (1)/Contents.003"),
                    prefab.GetChild("Bin (1)/Contents.004")
                };
                f_Items.SetValue(binView, items);
            }
        }
    }
}
