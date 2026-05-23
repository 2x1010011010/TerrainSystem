using TerrainSystem.Runtime.Data;
using TerrainSystem.Runtime.Rendering;
using UnityEditor;
using UnityEngine;

namespace TerrainSystem.Editor
{
    public static class CreateTerrainMenu
    {
        [MenuItem("Tools/Terrain System/Create Terrain")]
        public static void CreateTerrain()
        {
            TerrainDataAsset terrainData =
                LoadOrCreateTerrainData();

            Material terrainMaterial =
                LoadOrCreateTerrainMaterial();

            terrainData.TerrainMaterial =
                terrainMaterial;

            GameObject terrainRoot =
                new GameObject("Terrain");

            TerrainWorldRenderer renderer =
                terrainRoot.AddComponent<TerrainWorldRenderer>();

            renderer.SetTerrainData(terrainData);

            renderer.Generate();

            Selection.activeGameObject =
                terrainRoot;

            Debug.Log("Terrain generated.");
        }

        private static TerrainDataAsset LoadOrCreateTerrainData()
        {
            const string path =
                "Assets/TerrainSystem/Data/TerrainData.asset";

            TerrainDataAsset data =
                AssetDatabase.LoadAssetAtPath<TerrainDataAsset>(path);

            if (data != null)
                return data;

            CreateFolders();

            data =
                ScriptableObject.CreateInstance<TerrainDataAsset>();

            data.ChunksX = 4;
            data.ChunksZ = 4;

            data.ChunkResolution = 32;
            data.ChunkSize = 20f;

            data.HeightMultiplier = 10f;

            data.NoiseScale = 0.03f;
            data.Seed = Random.Range(0, 999999);

            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();

            Debug.Log("Created TerrainData.");

            return data;
        }

        private static Material LoadOrCreateTerrainMaterial()
        {
            const string path =
                "Assets/TerrainSystem/Data/TerrainMaterial.mat";

            Material material =
                AssetDatabase.LoadAssetAtPath<Material>(path);

            if (material != null)
                return material;

            CreateFolders();

            Shader shader =
                Shader.Find(
                    "Universal Render Pipeline/Simple Lit"
                );

            material =
                new Material(shader);

            material.SetFloat("_Smoothness", 0f);

            AssetDatabase.CreateAsset(material, path);
            AssetDatabase.SaveAssets();

            Debug.Log("Created Terrain Material.");

            return material;
        }

        private static void CreateFolders()
        {
            if (!AssetDatabase.IsValidFolder(
                    "Assets/TerrainSystem"))
            {
                AssetDatabase.CreateFolder(
                    "Assets",
                    "TerrainSystem");
            }

            if (!AssetDatabase.IsValidFolder(
                    "Assets/TerrainSystem/Data"))
            {
                AssetDatabase.CreateFolder(
                    "Assets/TerrainSystem",
                    "Data");
            }
        }
    }
}