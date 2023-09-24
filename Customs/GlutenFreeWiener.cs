using KitchenData;
using KitchenLib.Customs;
using KitchenLib.References;
using KitchenLib.Utils;
using System.Collections.Generic;

namespace YouAskedForIt.Customs
{
    public class GlutenFreeWiener : CustomDish
    {
        public override string UniqueNameID => "glutenFreeWiener";

        public override DishType Type => DishType.Main;

        public override List<Dish.MenuItem> ResultingMenuItems => new List<Dish.MenuItem>()
        {
            new Dish.MenuItem()
            {
                Item = (Item)GDOUtils.GetExistingGDO(-248200024),    // Hot Dog Cooked
                Phase = MenuPhase.Main,
                Weight = 1f
            }
        };

        public override HashSet<Item> MinimumIngredients => new HashSet<Item>()
        {
            (Item)GDOUtils.GetExistingGDO(1702717896)   // Hot Dog Raw
        };

        public override CardType CardType => CardType.Default;

        public override UnlockGroup UnlockGroup => UnlockGroup.Dish;

        public override DishCustomerChange CustomerMultiplier => DishCustomerChange.LargeIncrease;

        public override Unlock.RewardLevel ExpReward => Unlock.RewardLevel.Medium;

        public override bool IsUnlockable => true;

        public override List<(Locale, UnlockInfo)> InfoList => new List<(Locale, UnlockInfo)>()
        {
            (Locale.English, LocalisationUtils.CreateUnlockInfo("Gluten Free - Wiener", "Just a wiener...", ""))
        };

        public override Dictionary<Locale, string> Recipe => new Dictionary<Locale, string>()
        {
            { Locale.English, "Cook hot dog, serve" }
        };

        public override List<Unlock> HardcodedRequirements => new List<Unlock>()
        {
            (Unlock)GDOUtils.GetExistingGDO(1626323920) // Hot Dog Base
        };
    }
}
