using TerrainSystem.Runtime.Data;
using UnityEngine;

namespace TerrainSystem.Runtime.Core
{
  public static class TerrainBootstrap
  {
    public static bool Validate(TerrainDataAsset terrainData)
    {
      if (terrainData == null)
      {
        Debug.LogError("[TerrainSystem] TerrainDataAsset is null.");
        return false;
      }

      if (terrainData.NoiseSettings == null)
      {
        Debug.LogError("[TerrainSystem] NoiseSettingsAsset is missing.");
        return false;
      }

      if (terrainData.TerrainMaterial == null)
      {
        Debug.LogError("[TerrainSystem] Terrain material is missing.");
        return false;
      }

      if (terrainData.ChunkResolution < TerrainConstants.MinChunkResolution)
      {
        Debug.LogError("[TerrainSystem] Chunk resolution is too low.");
        return false;
      }

      if (terrainData.ChunkSize <= 0f)
      {
        Debug.LogError("[TerrainSystem] Chunk size must be greater than zero.");
        return false;
      }

      return true;
    }

    public static string GetChunkName(Vector2Int coordinate) =>
      $"{TerrainConstants.ChunkObjectPrefix}" + $"{coordinate.x}_{coordinate.y}";

    public static Vector2Int WorldToChunkCoordinate(Vector3 worldPosition, float chunkSize) =>
      new(Mathf.FloorToInt(worldPosition.x / chunkSize), Mathf.FloorToInt(worldPosition.z / chunkSize));

    public static Vector3 ChunkToWorldPosition(Vector2Int coordinate, float chunkSize) =>
      new(coordinate.x * chunkSize, 0f, coordinate.y * chunkSize);
  }
}