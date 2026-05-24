using TerrainSystem.Runtime.Data;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TerrainSystem.Runtime
{
  public static class TerrainAutoBootstrap
  {
    private const string FolderPath = "Assets/TerrainSystem/Data";
    private const string DataPath = "Assets/TerrainSystem/Data/TerrainData.asset";
    private const string MatPath = "Assets/TerrainSystem/Data/TerrainMaterial.mat";

    public static TerrainDataAsset GetOrCreateData()
    {
#if UNITY_EDITOR
      EnsureFolders();

      var data =
        AssetDatabase.LoadAssetAtPath<TerrainDataAsset>(DataPath);

      if (data != null)
        return data;

      data = ScriptableObject.CreateInstance<TerrainDataAsset>();

      data.ChunksX = 4;
      data.ChunksZ = 4;
      data.ChunkResolution = 32;
      data.ChunkSize = 20f;
      data.HeightMultiplier = 10f;
      data.NoiseScale = 0.03f;
      data.Seed = Random.Range(0, 999999);

      AssetDatabase.CreateAsset(data, DataPath);
      AssetDatabase.SaveAssets();

      Debug.Log("Created TerrainData at " + DataPath);

      return data;
#else
            return null;
#endif
    }

    public static Material GetOrCreateMaterial()
    {
#if UNITY_EDITOR
      string path = "Assets/TerrainSystem/Data/TerrainMaterial.mat";

      var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
      if (mat != null)
        return mat;

      Shader shader = Shader.Find("Universal Render Pipeline/Simple Lit");

      if (shader == null)
      {
        Debug.LogError("Built-in Standard shader not found!");
        shader = Shader.Find("Diffuse");
      }

      mat = new Material(shader);
      mat.SetFloat("_Smoothness", 0f);

      AssetDatabase.CreateAsset(mat, path);
      AssetDatabase.SaveAssets();

      Debug.Log("Created Terrain Material.");

      return mat;
#else
        return null;
#endif
    }

#if UNITY_EDITOR
    private static void EnsureFolders()
    {
      if (!AssetDatabase.IsValidFolder("Assets/TerrainSystem"))
      {
        AssetDatabase.CreateFolder("Assets", "TerrainSystem");
      }

      if (!AssetDatabase.IsValidFolder(FolderPath))
      {
        AssetDatabase.CreateFolder("Assets/TerrainSystem", "Data");
      }
    }
#endif
  }
}