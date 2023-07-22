using KitchenData;
using KitchenLib.Customs;
using KitchenLib.References;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace YouAskedForIt.Customs
{
    public class ServingBoardDirty : CustomItem
    {
        public override string UniqueNameID => "servingBoardDirty";

        public override GameObject Prefab => Main.Bundle.LoadAsset<GameObject>("Serving Board Dirty");

        public override List<Item.ItemProcess> Processes => new List<Item.ItemProcess>()
        {
            new Item.ItemProcess()
            {
                Process = GDOUtils.GetExistingGDO(ProcessReferences.Clean) as Process,
                Result = GDOUtils.GetExistingGDO(ItemReferences.ServingBoard) as Item,
                IsBad = false,
                Duration = 3,
                RequiresWrapper = false
            },
            new Item.ItemProcess()
            {
                Process = GDOUtils.GetExistingGDO(ProcessReferences.CleanSoak) as Process,
                Result = GDOUtils.GetExistingGDO(ItemReferences.ServingBoard) as Item,
                IsBad = false,
                Duration = 6,
                RequiresWrapper = false
            }
        };
        public override bool IsIndisposable => true;

        public override void SetupPrefab(GameObject prefab)
        {
            MaterialUtils.ApplyMaterial(prefab, "Wooden Serving Board", new Material[] { MaterialUtils.GetExistingMaterial("Wood - Barrel") });
            MaterialUtils.ApplyMaterial(prefab, "Plane", new Material[] { MaterialUtils.GetExistingMaterial("Plate - Dirty Food") });
            MaterialUtils.ApplyMaterial(prefab, "Plane.001", new Material[] { MaterialUtils.GetExistingMaterial("Plate - Dirty Food 2") });
        }
    }
}
