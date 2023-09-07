using HarmonyLib;
using Kitchen;
using KitchenData;
using System;
using System.Reflection;
using Unity.Entities;

namespace YouAskedForIt.Patches
{
    [HarmonyPatch]
    static class ImproveBlueprintAfterDuration_Patch
    {
        [HarmonyTargetMethod]
        static MethodBase TargetMethod()
        {
            Type type = AccessTools.FirstInner(typeof(ImproveBlueprintAfterDuration), t => t.Name.Contains($"c__DisplayClass_OnUpdate_LambdaJob0"));
            return AccessTools.FirstMethod(type, method => method.Name.Contains("OriginalLambdaBody"));
        }

        [HarmonyPrefix]
        static bool OriginalLambdaBody_Prefix(Entity desk, ref EntityCommandBuffer ___ecb, ref CDeskTarget target, ref CTakesDuration duration, in CModifyBlueprintStoreAfterDuration improvement, ref ComponentDataFromEntity<CBlueprintStore> ____ComponentDataFromEntity_CBlueprintStore_0)
        {
            if (!duration.Active || duration.Remaining > 0f || !PatchController.StaticRequire(target.Target, out CBlueprintStore comp) || !comp.InUse || !GameData.Main.TryGet(comp.ApplianceID, out Item _))
			{
                return true;
            }
            if (improvement.PerformCopy)
            {
                comp.HasBeenCopied = true;
            }
            if (PatchController.StaticRequire(target.Target, out CCabinetModifier comp2) && comp2.DisablesDeskAfterImprovement)
            {
                ___ecb.AddComponent<CIsBroken>(desk);
            }
            ____ComponentDataFromEntity_CBlueprintStore_0[target.Target] = comp;
            target.Target = default;
            return false;
        }
    }
}
