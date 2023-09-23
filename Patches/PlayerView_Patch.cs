using HarmonyLib;
using Kitchen;
using UnityEngine;

namespace YouAskedForIt.Patches
{
    [HarmonyPatch]
    static class PlayerView_Patch
    {
        [HarmonyPatch(typeof(PlayerView), "Update")]
        [HarmonyPostfix]
        static void Postfix(ref PlayerView __instance, ref PlayerView.ViewData ___Data, ref Vector3 ___MovementVector)
        {
            if (___Data.Speed > 1.101f && ___MovementVector != Vector3.zero)
            {

            }
        }
    }
}
