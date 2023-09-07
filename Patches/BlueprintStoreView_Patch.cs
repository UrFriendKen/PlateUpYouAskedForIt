using HarmonyLib;
using Kitchen;
using KitchenData;
using System.Linq;
using TMPro;
using UnityEngine;

namespace YouAskedForIt.Patches
{
    [HarmonyPatch]
    static class BlueprintStoreView_Patch
    {
        [HarmonyPatch(typeof(BlueprintStoreView), "UpdateData")]
        [HarmonyPostfix]
        static void UpdateData_Postfix(BlueprintStoreView.ViewData view_data, ref MeshRenderer ___Renderer, ref MeshRenderer ___CopyRenderer, ref TextMeshPro ___Title, TextMeshPro ___CopyTitle)
        {
            if (GameData.Main.TryGet(view_data.Appliance, out Item item))
            {
                if (item.Prefab != null)
                {
                    ___Renderer?.material.SetTexture("_Image", PrefabSnapshot.GetSnapshot(item.Prefab));
                    ___CopyRenderer?.material.SetTexture("_Image", PrefabSnapshot.GetSnapshot(item.Prefab));
                }
                string titleText = item.name.Split('.').Last();
                if (___Title != null)
                {
                    ___Title.text = titleText;
                }
                if (___CopyTitle != null)
                {
                    ___CopyTitle.text = titleText;
                }
            }
        }
    }
}
