﻿using HarmonyLib;
using Kitchen;
using Kitchen.Components;
using System.Collections.Generic;
using UnityEngine;
using YouAskedForIt.Utils;

namespace YouAskedForIt.Patches
{
    [HarmonyPatch]
    static class LocalViewRouter_Patch
    {
        static Transform Container;
        static Dictionary<ViewType, GameObject> Prefabs = new Dictionary<ViewType, GameObject>();

        [HarmonyPatch(typeof(LocalViewRouter), "GetPrefab")]
        [HarmonyPrefix]
        static bool GetPrefab_Prefix(ViewType view_type, ref GameObject __result)
        {
            if (Prefabs.TryGetValue(view_type, out GameObject cachedPrefab))
            {
                __result = cachedPrefab;
                return false;
            }

            if (Container == null)
            {
                GameObject containerGO = new GameObject("YouAskedForIt Container");
                containerGO.SetActive(false);
                Container = containerGO.transform;
            }

            if (view_type == Main.ExplosionEffectViewType)
            {
                GameObject explosionPrefab = Main.Bundle.LoadAsset<GameObject>("ExplosionParticleSystem");
                if (explosionPrefab != null)
                {
                    GameObject explosionInstance = GameObject.Instantiate(explosionPrefab);
                    explosionInstance.transform.SetParent(Container);

                    ExplosionView explosionView = explosionInstance.AddComponent<ExplosionView>();
                    explosionView.Explosion = explosionInstance.GetComponent<ParticleSystem>();

                    Prefabs.Add(view_type, explosionInstance);
                    __result = explosionInstance;
                    return false;
                }
            }

            if (view_type == Main.ExplosionEffectSoundViewType)
            {
                GameObject explosionSoundInstance = new GameObject("ExplosionSoundEffect");
                explosionSoundInstance.transform.SetParent(Container);

                ExplosionSoundView explosionSoundView = explosionSoundInstance.AddComponent<ExplosionSoundView>();
                SoundSource soundSource = explosionSoundInstance.AddComponent<SoundSource>();
                soundSource.TransitionTime = 0.1f;
                soundSource.ShouldLoop = false;
                explosionSoundView.SoundSource = soundSource;
                explosionSoundView.AudioClip = BundleUtils.LoadAudioClipFromAssetBundle(Main.Bundle, "WoodExplosion.wav");

                Prefabs.Add(view_type, explosionSoundInstance);
                __result = explosionSoundInstance;
                return false;
            }
            return true;
        }
    }
}