using HarmonyLib;
using Kitchen;
using System;
using System.Reflection;
using Unity.Entities;

namespace YouAskedForIt.Patches
{
    [HarmonyPatch]
    static class BringNextCustomerAfterDuration_Patch
    {

        [HarmonyTargetMethod]
        static MethodBase TargetMethod()
        {
            Type type = AccessTools.FirstInner(typeof(BringNextCustomerAfterDuration), t => t.Name.Contains($"c__DisplayClass_OnUpdate_LambdaJob0"));
            return AccessTools.FirstMethod(type, method => method.Name.Contains("OriginalLambdaBody"));
        }

        [HarmonyPostfix]
        static void OriginalLambdaBody_Postfix(Entity desk, in CTakesDuration duration)
        {
            if (Main.PrefManager.Get<bool>(Main.SIMPLICITY_BOOKING_DESK_ID) &&
                !PatchController.StaticHas<CIsBroken>(desk) &&
                duration.Active && duration.Remaining <= 0f &&
                PatchController.StaticRequireBuffer(desk, out DynamicBuffer<CAffectedBy> affectedBys))
            {
                for (int i = 0; i < affectedBys.Length; i++)
                {
                    CAffectedBy item = affectedBys[i];
                    if (PatchController.StaticRequire(item, out CCabinetModifier modifier) &&
                        modifier.DisablesDeskAfterImprovement &&
                        PatchController.StaticRequire(item, out CAppliesEffect appliesEffect) &&
                        appliesEffect.IsActive)
                    {
                        PatchController.StaticSet<CIsBroken>(desk);
                        break;
                    }
                }
            }
        }
    }
}
