using HarmonyLib;
using Kitchen;

namespace YouAskedForIt.Patches
{
    [HarmonyPatch]
    static class AssignmentSystem_Patch
    {
        [HarmonyPatch(typeof(AssignmentSystem), "NewAssignment")]
        [HarmonyPrefix]
        static bool NewAssignment_Prefix(CAvailableAssignment assignment)
        {
            return !PatchController.StaticHas<CPreventUse>(assignment.Entity);
        }
    }
}
