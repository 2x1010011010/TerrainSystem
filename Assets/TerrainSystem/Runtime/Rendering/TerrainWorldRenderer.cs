using TerrainSystem.Runtime.Data;
using UnityEngine;

namespace TerrainSystem.Runtime.Rendering
{
  public class TerrainWorldRenderer : MonoBehaviour
  {
    [SerializeField] private TerrainDataAsset terrainData;

    public void SetTerrainData(TerrainDataAsset data) => 
      terrainData = data;

    public void Generate()
    {
      Clear();

      for (int z = 0; z < terrainData.ChunksZ; z++)
      {
        for (int x = 0; x < terrainData.ChunksX; x++)
        {
          ChunkData chunk =
            GetOrCreateChunk(x, z);

          CreateChunkRenderer(chunk);
        }
      }
    }

    private ChunkData GetOrCreateChunk(int x, int z)
    {
      foreach (var chunk in terrainData.Chunks)
      {
        if (chunk.Coordinate.x == x &&
            chunk.Coordinate.y == z)
        {
          return chunk;
        }
      }

      ChunkData newChunk =
        CreateChunkData(x, z);

      terrainData.Chunks.Add(newChunk);

      return newChunk;
    }

    private ChunkData CreateChunkData(int chunkX, int chunkZ)
    {
      int resolution =
        terrainData.ChunkResolution;

      ChunkData chunk = new ChunkData();

      chunk.Coordinate =
        new Vector2Int(chunkX, chunkZ);

      int mapSize =
        (resolution + 1) * (resolution + 1);

      chunk.Heights =
        new float[mapSize];

      chunk.SplatMap =
        new Color[mapSize];

      chunk.BiomeMap =
        new int[mapSize];

      chunk.WaterMap =
        new bool[mapSize];

      for (int z = 0; z <= resolution; z++)
      {
        for (int x = 0; x <= resolution; x++)
        {
          int index =
            TerrainMapUtility.ToIndex(
              x,
              z,
              resolution
            );

          float worldX =
            x +
            chunkX * resolution;

          float worldZ =
            z +
            chunkZ * resolution;

          float noise =
            Mathf.PerlinNoise(
              (worldX + terrainData.Seed)
              * terrainData.NoiseScale,
              (worldZ + terrainData.Seed)
              * terrainData.NoiseScale
            );

          chunk.Heights[index] =
            noise *
            terrainData.HeightMultiplier;

          chunk.SplatMap[index] =
            Color.red;

          chunk.BiomeMap[index] = 0;
          chunk.WaterMap[index] = false;
        }
      }

      return chunk;
    }

    private void CreateChunkRenderer(
      ChunkData chunkData
    )
    {
      GameObject chunkObject =
        new GameObject(
          $"Chunk_{chunkData.Coordinate.x}_{chunkData.Coordinate.y}"
        );

      chunkObject.transform.SetParent(transform);

      float chunkSize =
        terrainData.ChunkSize;

      chunkObject.transform.position =
        new Vector3(
          chunkData.Coordinate.x * chunkSize,
          0,
          chunkData.Coordinate.y * chunkSize
        );

      TerrainChunkRenderer renderer =
        chunkObject.AddComponent<TerrainChunkRenderer>();

      renderer.Build(
        chunkData,
        terrainData
      );
    }

    public void Clear()
    {
      for (int i = transform.childCount - 1; i >= 0; i--)
      {
        DestroyImmediate(
          transform.GetChild(i).gameObject
        );
      }
    }
  }
}