using HarmonyLib;
using Kitchen;

namespace YouAskedForIt.Patches
{
    [HarmonyPatch]
    static class CustomerIndicatorView_Patch
    {
        [HarmonyPatch(typeof(CustomerIndicatorView), "SetBar")]
        [HarmonyPrefix]
        static void SetBar_Prefix(ref float fraction)
        {
            if (Main.PrefManager.Get<bool>(Main.REVERSE_PROGRESS_BARS_ID))
                fraction = 1f - fraction;
        }
    }
}
