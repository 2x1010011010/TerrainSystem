using TerrainSystem.Runtime.Data;
using UnityEditor;
using UnityEngine;

namespace TerrainSystem.Editor.Utilities
{
  public static class TerrainAssetCreator
  {
    private const string RootPath = "Assets/TerrainSystem/Data/";

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
      CreateDirectories();
      CreateAssets();
    }

    private static void CreateDirectories()
    {
      if (!AssetDatabase.IsValidFolder(RootPath))
        AssetDatabase.CreateFolder("Assets/TerrainSystem", "Data");
    }

    private static void CreateAssets()
    {
      CreateNoiseAsset();
      CreateTerrainAsset();
      CreateMaterial();
    }

    private static void CreateNoiseAsset()
    {
      string path = RootPath + "NoiseSettings.asset";

      if (AssetDatabase.LoadAssetAtPath<NoiseSettingsAsset>(path) != null) return;
      
      NoiseSettingsAsset asset = ScriptableObject.CreateInstance<NoiseSettingsAsset>();
      AssetDatabase.CreateAsset(asset, path);
    }

    private static void CreateTerrainAsset()
    {
      string path = RootPath + "TerrainData.asset";

      if (AssetDatabase.LoadAssetAtPath<TerrainDataAsset>(path) != null) return;

      TerrainDataAsset asset = ScriptableObject.CreateInstance<TerrainDataAsset>();
      AssetDatabase.CreateAsset(asset, path);
    }

    private static void CreateMaterial()
    {
      string path = RootPath + "Terrain.mat";

      if (AssetDatabase.LoadAssetAtPath<Material>(path) != null) return;

      Shader shader = Shader.Find("Universal Render Pipeline/Lit");
      Material material = new Material(shader);
      AssetDatabase.CreateAsset(material, path);
    }
  }
}