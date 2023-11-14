using HarmonyLib;
using Kitchen;
using Kitchen.Components;
using KitchenLib.Utils;
using System.Collections.Generic;
using Unity.Entities;
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

            if (view_type == Main.FlourEmitterViewType)
            {
                GameObject flourEmitterPrefab = Main.Bundle.LoadAsset<GameObject>("FlourEmitterParticleSystem");
                if (flourEmitterPrefab != null)
                {
                    GameObject flourEmitterInstance = GameObject.Instantiate(flourEmitterPrefab);
                    flourEmitterInstance.transform.SetParent(Container);

                    ParticleEmitterView flourEmitter = flourEmitterInstance.AddComponent<ParticleEmitterView>();
                    flourEmitter.Emitter = flourEmitterInstance.GetComponent<ParticleSystem>();

                    Prefabs.Add(view_type, flourEmitterInstance);
                    __result = flourEmitterInstance;
                    return false;
                }
            }

            if (view_type == Main.FogViewType)
            {
                GameObject fogPrefab = Main.Bundle.LoadAsset<GameObject>("Fog");
                GameObject fogPrefabInstance = fogPrefab != null ? GameObject.Instantiate(fogPrefab) : new GameObject("Backup Fog");
                fogPrefabInstance.transform.SetParent(Container);
                fogPrefabInstance.transform.Reset();

                GameObject errorFog = GameObject.CreatePrimitive(PrimitiveType.Cube);
                errorFog.name = "Fog Error";
                errorFog.transform.SetParent(fogPrefabInstance.transform);
                errorFog.transform.Reset();
                errorFog.transform.localPosition = Vector3.up * 0.9f;
                errorFog.transform.localScale = new Vector3(1.2f, 1.8f, 1.2f);
                errorFog.SetActive(false);
                
                Collider[] colliders = errorFog.GetComponentsInChildren<Collider>();
                for (int i = colliders.Length - 1; i > -1; i--)
                {
                    Component.DestroyImmediate(colliders[i]);
                }
                
                MaterialUtils.ApplyMaterial(errorFog, "", new Material[] { MaterialUtils.GetExistingMaterial("Plastic - Black Dark") });
                FogView fogView = fogPrefabInstance.AddComponent<FogView>();
                fogView.ErrorFog = errorFog;

                Prefabs.Add(view_type, fogPrefabInstance);
                __result = fogPrefabInstance;
                return false;
            }

            return true;
        }
    }
}
