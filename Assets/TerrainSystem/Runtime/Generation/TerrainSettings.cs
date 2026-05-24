using UnityEngine;

namespace TerrainSystem
{
    [CreateAssetMenu(fileName = "TerrainSettings", menuName = "Terrain System/Terrain Settings")]
    public class TerrainSettings : ScriptableObject
    {
        [Header("Mesh Dimensions")]
        public Vector2Int chunkCount = new Vector2Int(4, 4);
        public Vector2Int chunkResolution = new Vector2Int(16, 16);
        public Vector2 chunkSize = new Vector2(10f, 10f);

        [Header("Height")]
        public float heightScale = 5f;
        public float heightMultiplier = 1f;

        [Header("Noise")]
        public float noiseScale = 0.3f;
        public int noiseSeed = 42;
        public Vector2 noiseOffset = Vector2.zero;
        [Range(1, 8)]
        public int octaves = 4;
        [Range(0f, 1f)]
        public float persistence = 0.5f;
        [Range(1f, 4f)]
        public float lacunarity = 2f;

        [Header("Material")]
        public Material terrainMaterial;

        /// <summary>Total world-space size of the terrain (X, Z).</summary>
        public Vector2 TotalSize => new Vector2(
            chunkCount.x * chunkSize.x,
            chunkCount.y * chunkSize.y
        );
    }
}
