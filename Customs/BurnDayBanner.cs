using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace YouAskedForIt.Customs
{
    public class BurnDayBanner : CustomAppliance
    {
        public override string UniqueNameID => "burnDayBanner";

        public override GameObject Prefab => Main.Bundle.LoadAsset<GameObject>("Burn Day Banner");

        public override List<(Locale, ApplianceInfo)> InfoList => new List<(Locale, ApplianceInfo)>()
        {
            (Locale.English, LocalisationUtils.CreateApplianceInfo("Burn Day Banner", "", new List<Appliance.Section>(), new List<string>()))
        };

        public override PriceTier PriceTier => PriceTier.Free;

        public override bool IsPurchasable => false;

        public override void SetupPrefab(GameObject prefab)
        {
            MaterialUtils.ApplyMaterial(prefab, "BirthdayBanner/Text", new Material[] { MaterialUtils.GetExistingMaterial("Flour Bag") });
            MaterialUtils.ApplyMaterial(prefab, "BirthdayBanner/Banners", new Material[] { MaterialUtils.GetExistingMaterial("AppleRed") });
            MaterialUtils.ApplyMaterial(prefab, "BirthdayBanner/Cylinder", new Material[] { MaterialUtils.GetExistingMaterial("Wood - Default") });
        }
    }
}
