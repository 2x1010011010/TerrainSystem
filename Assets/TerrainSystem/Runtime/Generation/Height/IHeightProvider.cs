using UnityEngine;

namespace TerrainSystem.Runtime.Generation.Height
{
  public interface IHeightProvider
  {
    float GetHeight(float x, float z);
  }
}