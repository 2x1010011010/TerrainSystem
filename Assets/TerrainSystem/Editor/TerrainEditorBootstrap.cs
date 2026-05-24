#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace TerrainSystem.Editor
{
    /// <summary>
    /// Menu items under Tools > Terrain System.
    /// All data assets (TerrainSettings, Material) are created in Assets/TerrainSystem/Data/.
    /// </summary>
    static class TerrainMenuItems
    {
        const string k_DataFolder  = "Assets/TerrainSystem/Data";
        const string k_SettingsPath = k_DataFolder + "/TerrainSettings.asset";
        const string k_MaterialPath = k_DataFolder + "/TerrainMaterial.mat";

        // ------------------------------------------------------------------ //
        //  Menu entry
        // ------------------------------------------------------------------ //

        [MenuItem("Tools/Terrain System/Create Terrain", false, 10)]
        static void CreateTerrain()
        {
            EnsureDataFolder();

            TerrainSettings settings = GetOrCreateSettings();
            Material mat             = GetOrCreateMaterial();

            // Assign material to settings if not already set
            if (settings.terrainMaterial == null)
            {
                settings.terrainMaterial = mat;
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
            }

            // Create terrain GameObject
            var go = new GameObject("Terrain");
            Undo.RegisterCreatedObjectUndo(go, "Create Terrain");

            var gen = go.AddComponent<TerrainGenerator>();
            gen.settings = settings;

            // Assign material to renderer
            var mr = go.GetComponent<MeshRenderer>();
            mr.sharedMaterial = mat;

            gen.Generate();

            Selection.activeGameObject = go;
            EditorGUIUtility.PingObject(go);
        }

        // ------------------------------------------------------------------ //
        //  Asset helpers
        // ------------------------------------------------------------------ //

        static void EnsureDataFolder()
        {
            // Create intermediate folders one level at a time
            if (!AssetDatabase.IsValidFolder("Assets/TerrainSystem"))
                AssetDatabase.CreateFolder("Assets", "TerrainSystem");

            if (!AssetDatabase.IsValidFolder(k_DataFolder))
                AssetDatabase.CreateFolder("Assets/TerrainSystem", "Data");
        }

        static TerrainSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<TerrainSettings>(k_SettingsPath);
            if (settings != null) return settings;

            settings = ScriptableObject.CreateInstance<TerrainSettings>();
            AssetDatabase.CreateAsset(settings, k_SettingsPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"[TerrainSystem] Created TerrainSettings at {k_SettingsPath}");
            return settings;
        }

        static Material GetOrCreateMaterial()
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(k_MaterialPath);
            if (mat != null) return mat;

            // Prefer URP/Lit; fall back gracefully if URP is not installed
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                Debug.LogWarning("[TerrainSystem] 'Universal Render Pipeline/Lit' shader not found. " +
                                 "Is the URP package installed? Falling back to Standard.");
                shader = Shader.Find("Standard");
            }

            mat = new Material(shader) { name = "TerrainMaterial" };
            AssetDatabase.CreateAsset(mat, k_MaterialPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"[TerrainSystem] Created TerrainMaterial at {k_MaterialPath}");
            return mat;
        }
    }
}
#endif
