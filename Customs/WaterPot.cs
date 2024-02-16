using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace YouAskedForIt.Customs
{
    public class WaterPot : CustomItemGroup
    {
        public override string UniqueNameID => "waterPot";

        public override GameObject Prefab => ((Item)GDOUtils.GetExistingGDO(2141493703))?.Prefab;   // Broccoli Pot

        public override List<ItemGroup.ItemSet> Sets => new List<ItemGroup.ItemSet>()
        {
            new ItemGroup.ItemSet()
            {
                IsMandatory = true,
                Min = 1,
                Max = 1,
                Items = new List<Item>()
                {
                    (Item)GDOUtils.GetExistingGDO(-486398094)   // Pot
                }
            },
            new ItemGroup.ItemSet()
            {
                IsMandatory = true,
                Min = 1,
                Max = 1,
                Items = new List<Item>()
                {
                    (Item)GDOUtils.GetExistingGDO(1657174953)   // Water
                }
            }
        };

        public override List<Item.ItemProcess> Processes => new List<Item.ItemProcess>()
        {
            new Item.ItemProcess()
            {
                Duration = 3,
                IsBad = true,
                Process = (Process)GDOUtils.GetExistingGDO(1972879238), // Cook
                Result = (Item)GDOUtils.GetExistingGDO(-486398094)  // Pot
            }
        };
    }
}
