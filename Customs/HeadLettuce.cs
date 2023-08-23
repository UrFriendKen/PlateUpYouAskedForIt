using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;

namespace YouAskedForIt.Customs
{
    public class HeadLettuce : CustomDish
    {
        public override string UniqueNameID => "headLettuce";

        public override DishType Type => DishType.Main;

        public override List<Dish.MenuItem> ResultingMenuItems => new List<Dish.MenuItem>()
        {
            new Dish.MenuItem()
            {
                Item = (Item)GDOUtils.GetExistingGDO(-65594226),    // Lettuce
                Phase = MenuPhase.Main,
                Weight = 1f
            }
        };

        public override HashSet<Item> MinimumIngredients => new HashSet<Item>()
        {
            (Item)GDOUtils.GetExistingGDO(-65594226)    // Lettuce
        };

        public override CardType CardType => CardType.Default;

        public override UnlockGroup UnlockGroup => UnlockGroup.Dish;

        public override DishCustomerChange CustomerMultiplier => DishCustomerChange.LargeIncrease;

        public override Unlock.RewardLevel ExpReward => Unlock.RewardLevel.Medium;

        public override bool IsUnlockable => true;

        public override List<(Locale, UnlockInfo)> InfoList => new List<(Locale, UnlockInfo)>()
        {
            (Locale.English, LocalisationUtils.CreateUnlockInfo("Head Lettuce", "Adds Lettuce as a main", ""))
        };

        public override Dictionary<Locale, string> Recipe => new Dictionary<Locale, string>()
        {
            { Locale.English, "Give lettuce to customer" }
        };

        public override List<Unlock> HardcodedRequirements => new List<Unlock>()
        {
            (Unlock)GDOUtils.GetExistingGDO(1356267749) // Salad Base
        };
    }
}
