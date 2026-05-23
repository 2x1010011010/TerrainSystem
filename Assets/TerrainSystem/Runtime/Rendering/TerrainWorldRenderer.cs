using System.Collections;
using System.Collections.Generic;
using TerrainSystem.Runtime.Data;
using TerrainSystem.Runtime.Generation;
using UnityEngine;

namespace TerrainSystem.Runtime.Rendering
{
  public class TerrainWorldRenderer : MonoBehaviour
  {
    [SerializeField] private TerrainDataAsset terrainData;

    private readonly Dictionary<Vector2Int, TerrainChunkRenderer> chunks
      = new();

    private readonly HashSet<Vector2Int> dirtyChunks
      = new();

    private Coroutine generationRoutine;

    #region PUBLIC API

    public void SetTerrainData(TerrainDataAsset data)
    {
      terrainData = data;
    }

    public TerrainDataAsset TerrainData => terrainData;

    public void Generate()
    {
      if (terrainData == null)
        return;

      StopGeneration();

      MarkAllChunksDirty();
      generationRoutine = StartCoroutine(GenerateAllChunks());
    }

    public void RegenerateChunk(Vector2Int coord)
    {
      if (!chunks.ContainsKey(coord))
        return;

      if (generationRoutine != null)
        StopCoroutine(generationRoutine);

      generationRoutine = StartCoroutine(GenerateChunk(coord));
    }

    public void Clear()
    {
      foreach (var c in chunks.Values)
      {
        if (c != null)
          DestroyImmediate(c.gameObject);
      }

      chunks.Clear();
      dirtyChunks.Clear();
    }

    #endregion

    #region GENERATION

    private IEnumerator GenerateAllChunks()
    {
      for (int z = 0; z < terrainData.ChunksZ; z++)
      {
        for (int x = 0; x < terrainData.ChunksX; x++)
        {
          Vector2Int coord = new Vector2Int(x, z);

          yield return GenerateChunk(coord);
        }
      }
    }

    private IEnumerator GenerateChunk(Vector2Int coord)
    {
      ChunkData data = GetOrCreateChunkData(coord);

      TerrainChunkRenderer renderer = GetOrCreateRenderer(coord, data);

      int res = terrainData.ChunkResolution;

      for (int z = 0; z <= res; z++)
      {
        for (int x = 0; x <= res; x++)
        {
          int index = ToIndex(x, z, res);

          float worldX = x + coord.x * res;
          float worldZ = z + coord.y * res;

          float height =
            TerrainNoiseGenerator.GenerateHeight(
              worldX,
              worldZ,
              terrainData.NoiseScale,
              terrainData.HeightMultiplier,
              terrainData.Seed,
              terrainData.HeightCurve
            );

          data.Heights[index] = height;
        }

        if (z % 4 == 0)
          yield return null; // 💡 avoids editor freeze
      }

      renderer.Build(data, terrainData);
    }

    #endregion

    #region CHUNK DATA

    private ChunkData GetOrCreateChunkData(Vector2Int coord)
    {
      foreach (var c in terrainData.Chunks)
      {
        if (c.Coordinate == coord)
          return c;
      }

      ChunkData chunk = CreateChunk(coord);
      terrainData.Chunks.Add(chunk);

      return chunk;
    }

    private ChunkData CreateChunk(Vector2Int coord)
    {
      int res = terrainData.ChunkResolution;

      int size = (res + 1) * (res + 1);

      return new ChunkData
      {
        Coordinate = coord,
        Heights = new float[size],
        SplatMap = new Color[size],
        BiomeMap = new int[size],
        WaterMap = new bool[size]
      };
    }

    #endregion

    #region RENDERERS

    private TerrainChunkRenderer GetOrCreateRenderer(
      Vector2Int coord,
      ChunkData data)
    {
      if (chunks.TryGetValue(coord, out var existing))
        return existing;

      GameObject go = new GameObject($"Chunk_{coord.x}_{coord.y}");
      go.transform.SetParent(transform);

      go.transform.position = new Vector3(
        coord.x * terrainData.ChunkSize,
        0,
        coord.y * terrainData.ChunkSize
      );

      var renderer = go.AddComponent<TerrainChunkRenderer>();

      chunks.Add(coord, renderer);

      return renderer;
    }

    #endregion

    #region DIRTY SYSTEM

    private void MarkAllChunksDirty()
    {
      dirtyChunks.Clear();

      for (int z = 0; z < terrainData.ChunksZ; z++)
      for (int x = 0; x < terrainData.ChunksX; x++)
        dirtyChunks.Add(new Vector2Int(x, z));
    }

    #endregion

    #region UTILS

    private int ToIndex(int x, int z, int res)
    {
      return z * (res + 1) + x;
    }

    private void StopGeneration()
    {
      if (generationRoutine != null)
      {
        StopCoroutine(generationRoutine);
        generationRoutine = null;
      }
    }

    #endregion

    #region UNITY EDITOR HOOK

#if UNITY_EDITOR
    private void OnValidate()
    {
      if (!Application.isPlaying && terrainData != null)
      {
        Generate();
      }
    }
#endif

    #endregion
  }
}