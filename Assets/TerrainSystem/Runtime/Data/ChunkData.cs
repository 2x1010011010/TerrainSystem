using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainSystem.Runtime.Data
{
  [Serializable]
  public class ChunkData
  {
    public Vector2Int Coordinate;
    public float[] Heights;
    public Color[] SplatMap;
    public int[] BiomeMap;
    public bool[] WaterMap;
    public List<VegetationInstance> Vegetation = new();
  }
}