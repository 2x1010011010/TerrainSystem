using UnityEngine;

namespace TerrainSystem.Runtime.Data
{
  [System.Serializable]
  public class TerrainChunkData
  {
    public Vector2Int Coord;

    public float[] Heights;
  }
}