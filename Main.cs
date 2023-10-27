using Kitchen;
using KitchenData;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.References;
using KitchenLib.Utils;
using KitchenMods;
using PreferenceSystem;
using System.Collections.Generic;
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
        public const string MOD_VERSION = "0.1.19";
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
        public const string DESTROY_PROTECTORS_ON_FIRE_ID = "destroyProtectorsOnFire";
        public const string SOUND_EFFECTS_EXPLOSION_VOLUME_ID = "soundEffectsExplosionVolume";
        public const string REHEARSAL_TIME_ID = "rehearsalTime";
        public const string RANDOMLY_ROTATE_ICE_CREAM_ID = "randomlyRotateIceCream";
        public const string REVERSE_PROGRESS_BARS_ID = "reverseProgressBars";
        public const string SIMPLICITY_BOOKING_DESK_ID = "simplicityBookingDesk";
        public const string FOG_OF_WAR_ID = "fogOfWar";
        public const string FOG_OF_WAR_SAME_ROOM_RADIUS_ID = "fogOfWarSameRoomRadius";
        public const string FOG_OF_WAR_OTHER_ROOM_RADIUS_ID = "fogOfWarOtherRoomRadius";
        internal const string CUSTOM_PRACTICE_MODE_TEXT = "Rehearsal Time";
        internal static readonly ViewType ExplosionEffectViewType = (ViewType)HashUtils.GetInt32HashCode($"{MOD_GUID}:ExplosionEffect");
        internal static readonly ViewType ExplosionEffectSoundViewType = (ViewType)HashUtils.GetInt32HashCode($"{MOD_GUID}:ExplosionEffectSound");
        internal static readonly ViewType FlourEmitterViewType = (ViewType)HashUtils.GetInt32HashCode($"{MOD_GUID}:FlourEmitter");
        internal static readonly ViewType FogViewType = (ViewType)HashUtils.GetInt32HashCode($"{MOD_GUID}:FogView");

        protected override void OnInitialise()
        {
        }

        private void AddGameData()
        {
            LogInfo("Attempting to register game data...");

            _servingBoardDirty = AddGameDataObject<ServingBoardDirty>();
            AddGameDataObject<KuluBin>();
            AddGameDataObject<HeadLettuce>();
            AddGameDataObject<GlutenFreeWiener>();
            AddGameDataObject<BurnDayBanner>();

            LogInfo("Done loading game data.");
        }

        protected override void OnUpdate()
        {
        }

        private HashSet<int> AppliancesDestroyIfOnFireAtNight = new HashSet<int>()
        {
            -648349801, // Rug
            591400026,  // Cosy Rug
            2076966627, // Fancy Rug
            1765889988  // Floor Protector
        };

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
                .AddLabel("You Asked For It!")
                .AddSpacer()
                .AddConditionalBlocker(() => Session.CurrentGameNetworkMode != GameNetworkMode.Host)
                .AddLabel("Wrong Delivery Breaks Table")
                .AddOption<bool>(
                    WRONG_DELIVERY_EXPLOSION_ID,
                    false,
                    new bool[] { false, true },
                    new string[] { "Disabled", "Enabled" })
                .AddLabel("Destroy Rugs and Floor Protectors If On Fire At End of Day")
                .AddOption<bool>(
                    DESTROY_PROTECTORS_ON_FIRE_ID,
                    false,
                    new bool[] { false, true },
                    new string[] { "Disabled", "Enabled" })
                .AddLabel("Randomly Rotate Ice Cream")
                .AddOption<bool>(
                    RANDOMLY_ROTATE_ICE_CREAM_ID,
                    false,
                    new bool[] { false, true },
                    new string[] { "Disabled", "At Start Of Day" })
                .AddLabel("Booking Desk Affected By Simplicity")
                .AddOption<bool>(
                    SIMPLICITY_BOOKING_DESK_ID,
                    false,
                    new bool[] { false, true },
                    new string[] { "Disabled", "Enabled" })
                .AddSpacer()
                .AddSubmenu("Fog Of War", "fogOfwar")
                    .AddLabel("Fog Of War")
                    .AddOption<bool>(
                        FOG_OF_WAR_ID,
                        false,
                        new bool[] { false, true },
                        new string[] { "Disabled", "Enabled" })
                    .AddLabel("Same Room Radius")
                    .AddOption<float>(
                        FOG_OF_WAR_SAME_ROOM_RADIUS_ID,
                        3f,
                        new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f, 4.5f, 5f },
                        new string[] { "1", "1.5", "2", "2.5", "3", "3.5", "4", "4.5", "5" })
                    .AddLabel("Other Room Radius")
                    .AddOption<float>(
                        FOG_OF_WAR_OTHER_ROOM_RADIUS_ID,
                        1.5f,
                        new float[] { 0f, 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f, 4.5f, 5f },
                        new string[] { "0", "1", "1.5", "2", "2.5", "3", "3.5", "4", "4.5", "5" })
                .SubmenuDone()
                .AddLabel("Serving Board Requires Washing")
                .AddInfo("Requires restart to take effect")
                .AddOption<bool>(
                    SERVING_BOARD_WASHING_ID,
                    false,
                    new bool[] { false, true },
                    new string[] { "Disabled", "Enabled" })
                .AddSpacer()
                .ConditionalBlockerDone()
                .AddSubmenu("Client Settings", "clientSettings")
                    .AddLabel("Client Settings")
                    .AddLabel("Table Explosion Volume")
                    .AddOption<float>(
                        SOUND_EFFECTS_EXPLOSION_VOLUME_ID,
                        0.5f,
                        new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f },
                        new string[] { "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%" })
                    .AddLabel("Practice Mode Text")
                    .AddOption<bool>(
                        REHEARSAL_TIME_ID,
                        false,
                        new bool[] { false, true },
                        new string[] { "Default", "It's Rehearsal Time!" })
                    .AddLabel("Reverse Patience/Progress Bars >:)")
                    .AddOption<bool>(
                        REVERSE_PROGRESS_BARS_ID,
                        false,
                        new bool[] { false, true },
                        new string[] { "Disabled", "Enabled" })
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

                    foreach (int applianceID in AppliancesDestroyIfOnFireAtNight)
                    {
                        if (args.gamedata.TryGet(applianceID, out Appliance appliance, warn_if_fail: true))
                        {
                            if (!appliance.Properties.Select(x => x.GetType()).Contains(typeof(CDestroyIfOnFireAtNight)))
                            {
                                appliance.Properties.Add(new CDestroyIfOnFireAtNight());
                            }
                        }
                    }
                }

                if (args.gamedata.TryGet(-110929446, out Item scrubbingBrush))
                {
                    scrubbingBrush.Prefab = Bundle.LoadAsset<GameObject>("Potato Brush");
                    MaterialUtils.ApplyMaterial(scrubbingBrush.Prefab, "ScrubbingBrush", new Material[] { MaterialUtils.GetExistingMaterial("Plastic - Blue"), MaterialUtils.GetExistingMaterial("Mop") });
                    MaterialUtils.ApplyMaterial(scrubbingBrush.Prefab, "Potato", new Material[] { MaterialUtils.GetExistingMaterial("Raw Potato - Skin") });
                    MaterialUtils.ApplyMaterial(scrubbingBrush.Prefab, "FaceAnchor/TootieFace/Face", new Material[] { MaterialUtils.GetExistingMaterial("Plastic - Black") });
                    MaterialUtils.ApplyMaterial(scrubbingBrush.Prefab, "FaceAnchor/TootieFace/Tongue", new Material[] { MaterialUtils.GetExistingMaterial("Plastic - Red") });
                }

                int[] makeStackable = new int[]
                {
                    -1660145659,    // Bin Bag
                    895813906       // Flammable Bin Bag
                };
                foreach (int id in makeStackable)
                {
                    if (args.gamedata.TryGet(id, out Item item))
                    {
                        item.ItemStorageFlags |= ItemStorage.StackableFood;
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
