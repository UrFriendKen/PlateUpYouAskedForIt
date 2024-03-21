using Kitchen;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using YouAskedForIt.Utils;
using KitchenMods;

namespace YouAskedForIt
{
    [FilterModes(AllowedModes = GameConnectionMode.All)]
    public class DoorColorChanger : GenericSystemBase, IModSystem
    {
        bool _isInit = false;
        HashSet<Material> _doorMaterials = new HashSet<Material>();
        Color _defaultColor = default;
        FixedColor _previousColor = FixedColor.Default;

        static Shader _simpleFlatShader = Shader.Find("Simple Flat");

        static readonly Dictionary<ViewType, string> LayoutViewDoorPaths = new Dictionary<ViewType, string>()
        {
            { ViewType.Floorplan, "Door/Door(Clone)/Cube" },
            { ViewType.FranchiseFloorplan, "Door/Franchise Door/Cube" },
        };

        static FieldInfo f_Prefabs = typeof(LayoutView).GetField("Prefabs", BindingFlags.NonPublic | BindingFlags.Instance);

        const string COLOR_PROPERTY_NAME = "_Color0";

        protected override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnUpdate()
        {
            if (!_isInit)
            {
                AssetDirectory assetDirectory = AssetDirectory;
                if (assetDirectory == null)
                    return;

                _isInit = true;

                foreach (KeyValuePair<ViewType, string> layoutViewDoorPath in LayoutViewDoorPaths)
                {
                    ViewType layoutViewType = layoutViewDoorPath.Key;
                    string path = layoutViewDoorPath.Value;

                    if (!assetDirectory.ViewPrefabs.TryGetValue(layoutViewType, out GameObject prefab))
                    {
                        Main.LogError($"Could not find prefab for ViewType.{layoutViewType}!");
                        continue;
                    }
                    LayoutView floorplanPrefabLayoutView = prefab.GetComponent<LayoutView>();
                    if (floorplanPrefabLayoutView == null)
                    {
                        Main.LogError($"ViewType.{layoutViewType} prefab does not have LayoutView component!");
                        continue;
                    }
                    if (!TryGetDefaultDoorMaterial(floorplanPrefabLayoutView, path, out Material doorMaterial))
                    {
                        Main.LogError($"Failed to get Door material for ViewType.{layoutViewType}!");
                        continue;
                    }
                    _defaultColor = doorMaterial.GetColor(COLOR_PROPERTY_NAME);
                    _doorMaterials.Add(doorMaterial);
                }
            }

            FixedColor prefValue = Main.PrefManager.Get<FixedColor>(Main.INSIDE_DOOR_COLOR_ID);
            if (_previousColor == prefValue)
                return;
            _previousColor = prefValue;
            Color newColor = prefValue == FixedColor.Default ? _defaultColor : ColorUtils.FromFixedColor(prefValue);
            UpdateDoorColor(newColor);
        }

        private void UpdateDoorColor(Color newColor)
        {
            Main.LogWarning($"Change door color: ({newColor.r}, {newColor.g}, {newColor.b}, {newColor.a})");
            foreach (Material material in _doorMaterials)
            {
                if (material.shader != _simpleFlatShader && _simpleFlatShader != null)
                    continue;

                material.SetColor(COLOR_PROPERTY_NAME, newColor);
            }
        }

        private bool TryGetDefaultDoorMaterial(LayoutView layoutView, string path, out Material material)
        {
            material = default;
            if (f_Prefabs == null)
            {
                Main.LogError("Could not find LayoutView.Prefabs!");
                return false;
            }

            LayoutPrefabSet layoutPrefabSet = (LayoutPrefabSet)f_Prefabs.GetValue(layoutView);
            if (!layoutPrefabSet ||
                !layoutPrefabSet.DoorPrefab)
            {
                Main.LogError("Could not get Door prefab!");
                return false;
            }

            Transform child = layoutPrefabSet.DoorPrefab.transform;

            foreach (string childName in path.Split('/'))
            {
                child = child?.Find(childName);
            }

            Material doorMaterial = child?.GetComponent<MeshRenderer>()?.material;
            if (!doorMaterial)
            {
                return false;
            }

            material = doorMaterial;
            return true;
        }

        private void ModifyDoorColors(LayoutView layoutView)
        {

        }
    }
}
