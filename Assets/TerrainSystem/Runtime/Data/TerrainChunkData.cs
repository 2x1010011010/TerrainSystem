using Unity.Collections;
using UnityEngine;
using System;

namespace TerrainSystem.Runtime.Data
{
  public sealed class TerrainChunkData : IDisposable
  {
    public Vector2Int Coordinate { get; }
    public Bounds Bounds { get; }
    public NativeArray<float> Heights { get; }
    public int Resolution { get; }
    public float Size { get; }

    public TerrainChunkData(Vector2Int coordinate, Bounds bounds, NativeArray<float> heights, int resolution, float size)
    {
      Coordinate = coordinate;
      Bounds = bounds;
      Heights = heights;
      Resolution = resolution;
      Size = size;
    }

    public float GetHeight(int x, int z)
    {
      int index = z * Resolution + x;
      return Heights[index];
    }

    public void Dispose()
    {
      if (Heights.IsCreated)
        Heights.Dispose();
    }
  }
}