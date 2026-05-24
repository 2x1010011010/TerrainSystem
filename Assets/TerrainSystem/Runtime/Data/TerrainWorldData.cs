using System.Collections.Generic;

namespace TerrainSystem.Runtime.Data
{
  [System.Serializable]
  public class TerrainWorldData
  {
    public List<TerrainChunkData> Chunks =
      new();
  }
}