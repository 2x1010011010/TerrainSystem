using System.Collections.Generic;
using TerrainSystem.Runtime.Core;
using TerrainSystem.Runtime.Data;
using TerrainSystem.Runtime.Generation.Height;
using TerrainSystem.Runtime.Generation.Noise;
using TerrainSystem.Runtime.Mesh;
using UnityEngine;

namespace TerrainSystem.Runtime.Streaming
{
  public sealed class TerrainStreamer
  {
    private readonly Dictionary<Vector2Int, TerrainChunk> _chunks = new();
    private readonly Transform _parent;
    private readonly Transform _target;
    private readonly TerrainDataAsset _settings;
    private readonly IHeightProvider _heightProvider;

    public TerrainStreamer(Transform parent, Transform target, TerrainDataAsset settings)
    {
      _parent = parent;
      _target = target;
      _settings = settings;

      INoiseGenerator noise = new PerlinNoiseGenerator(settings.NoiseSettings);
      _heightProvider = new TerrainHeightProvider(noise, settings);

      GenerateInitial();
    }

    public void Tick() =>
      UpdateVisibleChunks();

    public void Dispose()
    {
      foreach (TerrainChunk chunk in _chunks.Values)
        Object.DestroyImmediate(chunk.GameObject);

      _chunks.Clear();
    }

    public void Regenerate()
    {
      Dispose();
      GenerateInitial();
    }

    private void GenerateInitial() => 
      UpdateVisibleChunks();

    private void UpdateVisibleChunks()
    {
      if (_target == null) return;

      Vector2Int currentChunk = new Vector2Int(Mathf.FloorToInt(_target.position.x / _settings.ChunkSize), Mathf.FloorToInt(_target.position.z / _settings.ChunkSize));

      for (int y = -_settings.VisibleRadius; y <= _settings.VisibleRadius; y++)
      {
        for (int x = -_settings.VisibleRadius; x <= _settings.VisibleRadius; x++)
        {
          Vector2Int coordinate = currentChunk + new Vector2Int(x, y);

          if (_chunks.ContainsKey(coordinate)) continue;

          CreateChunk(coordinate);
        }
      }
    }

    private void CreateChunk(Vector2Int coordinate)
    {
      UnityEngine.Mesh mesh = MeshGenerator.Generate(coordinate, _settings, _heightProvider);

      GameObject chunkObject = new GameObject($"Chunk_{coordinate.x}_{coordinate.y}");

      chunkObject.transform.parent = _parent;
      chunkObject.transform.position = new Vector3(coordinate.x * _settings.ChunkSize, 0f, coordinate.y * _settings.ChunkSize);

      MeshFilter filter = chunkObject.AddComponent<MeshFilter>();
      MeshRenderer renderer = chunkObject.AddComponent<MeshRenderer>();
      MeshCollider collider = chunkObject.AddComponent<MeshCollider>();

      filter.sharedMesh = mesh;
      collider.sharedMesh = mesh;
      renderer.sharedMaterial = _settings.TerrainMaterial;

      TerrainChunk chunk = new TerrainChunk(coordinate, chunkObject);

      _chunks.Add(coordinate, chunk);
    }
  }
}