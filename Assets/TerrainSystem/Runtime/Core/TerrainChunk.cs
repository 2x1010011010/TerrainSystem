using UnityEngine;

namespace TerrainSystem.Runtime.Core
{
  public sealed class TerrainChunk
  {
    public Vector2Int Coordinate { get; }
    public GameObject GameObject { get; }

    public TerrainChunk(Vector2Int coordinate, GameObject gameObject)
    {
      Coordinate = coordinate;
      GameObject = gameObject;
    }
  }
}