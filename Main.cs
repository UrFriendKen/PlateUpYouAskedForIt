using Kitchen;
using KitchenData;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.References;
using KitchenMods;
using PreferenceSystem;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using YouAskedForIt.Customs;
using YouAskedForIt.Utils;

// Namespace should have "Kitchen" in the beginning
namespace YouAskedForIt
{
    public class Main : BaseMod, IModSystem
    {
        // GUID must be unique and is recommended to be in reverse domain name notation
        // Mod Name is displayed to the player and listed in the mods menu
        // Mod Version must follow semver notation e.g. "1.2.3"
        public const string MOD_GUID = "IcedMilo.PlateUp.YouAskedForIt";
        public const string MOD_NAME = "You Asked For It!";
        public const string MOD_VERSION = "0.1.6";
        public const string MOD_AUTHOR = "IcedMilo";
        public const string MOD_GAMEVERSION = ">=1.1.6";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.3" current and all future
        // e.g. ">=1.1.3 <=1.2.3" for all from/until

        public static AssetBundle Bundle;

        internal static PreferenceSystemManager PrefManager;

        public Main() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        public const string SERVING_BOARD_WASHING_ID = "servingBoardWashing";
        static ServingBoardDirty _servingBoardDirty;

        public const string WRONG_DELIVERY_EXPLOSION_ID = "wrongDeliveryExplosion";
        public const string SOUND_EFFECTS_EXPLOSION_VOLUME_ID = "soundEffectsExplosionVolume";
        internal static readonly ViewType ExplosionEffectViewType = (ViewType)HashUtils.GetInt32HashCode($"{MOD_GUID}:ExplosionEffect");
        internal static readonly ViewType ExplosionEffectSoundViewType = (ViewType)HashUtils.GetInt32HashCode($"{MOD_GUID}:ExplosionEffectSound");
        internal static readonly ViewType FlourEmitterViewType = (ViewType)HashUtils.GetInt32HashCode($"{MOD_GUID}:FlourEmitter");

        protected override void OnInitialise()
        {
        }

        private void AddGameData()
        {
            LogInfo("Attempting to register game data...");

            _servingBoardDirty = AddGameDataObject<ServingBoardDirty>();
            AddGameDataObject<KuluBin>();
            AddGameDataObject<HeadLettuce>();

            LogInfo("Done loading game data.");
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
            // TODO: Uncomment the following if you have an asset bundle.
            // TODO: Also, make sure to set EnableAssetBundleDeploy to 'true' in your ModName.csproj

            LogInfo("Attempting to load asset bundle...");
            Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).First();
            LogInfo("Done loading asset bundle.");

            // Register custom GDOs
            AddGameData();

            PrefManager = new PreferenceSystemManager(MOD_GUID, MOD_NAME);
            PrefManager
                .AddLabel("Serving Board Requires Washing")
                .AddInfo("Requires restart to take effect")
                .AddOption<bool>(
                    SERVING_BOARD_WASHING_ID,
                    false,
                    new bool[] { false, true },
                    new string[] { "Disabled", "Enabled" })

                .AddLabel("Wrong Velocities Delivery Breaks Table")
                .AddOption<bool>(
                    WRONG_DELIVERY_EXPLOSION_ID,
                    false,
                    new bool[] { false, true },
                    new string[] { "Disabled", "Enabled" })
                
                .AddSpacer()
                .AddSubmenu("Sound Effects Volume", "soundEffectsVolume")
                    .AddLabel("Table Explosion")
                    .AddOption<float>(
                        SOUND_EFFECTS_EXPLOSION_VOLUME_ID,
                        0.5f,
                        new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f },
                        new string[] { "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%" })
                    .AddSpacer()
                    .AddSpacer()
                .SubmenuDone()
                .AddSpacer()
                .AddSpacer();

            PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);

            // Perform actions when game data is built
            Events.BuildGameDataEvent += delegate (object s, BuildGameDataEventArgs args)
            {
                if (PrefManager.Get<bool>(SERVING_BOARD_WASHING_ID))
                {
                    foreach (Item item in args.gamedata.Get<Item>())
                    {
                        if ((item.DirtiesTo?.ID ?? 0) == ItemReferences.ServingBoard)
                        {
                            item.DirtiesTo = _servingBoardDirty.GameDataObject;
                        }
                    }
                }

                if (args.gamedata.TryGet(-233558851, out Appliance garageDecorations))
                {
                    if (!garageDecorations.Properties.Select(x => x.GetType()).Contains(typeof(CCrateCountMarker)))
                    {
                        garageDecorations.Properties.Add(new CCrateCountMarker());
                    }

                    string crateCountTextName = "Crate Count Text";
                    Transform gameObjectTransform = garageDecorations.Prefab?.transform.Find("GameObject");
                    if (gameObjectTransform != null && gameObjectTransform.Find(crateCountTextName) == null)
                    {
                        GameObject loadingBayText = gameObjectTransform.Find("Loading Bay Text")?.gameObject;
                        if (loadingBayText != null)
                        {
                            GameObject crateCountText = GameObject.Instantiate(loadingBayText);
                            crateCountText.name = crateCountTextName;
                            crateCountText.transform.SetParent(gameObjectTransform.transform, false);
                            crateCountText.transform.Reset();
                            crateCountText.transform.localPosition = new Vector3(1.2f, 0.06f, -1.7f);
                            crateCountText.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                            crateCountText.transform.localScale = Vector3.one * 0.01f;

                            AutoGlobalLocal autoGlobalLocal = crateCountText.GetComponent<AutoGlobalLocal>();
                            if (autoGlobalLocal != null)
                            {
                                Component.DestroyImmediate(autoGlobalLocal);
                            }

                            CrateCountView crateCountView = crateCountText.AddComponent<CrateCountView>();
                            TextMeshPro tmp = crateCountText.GetComponent<TextMeshPro>();
                            if (tmp != null)
                            {
                                crateCountView.Text = tmp;
                                tmp.text = "";
                            }
                        }
                    }
                }
            };
        }
        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
