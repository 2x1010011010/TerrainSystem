using TerrainSystem.Runtime.Data;
using UnityEngine;

namespace TerrainSystem.Runtime.Generation.Height
{
  public static class HeightClipper
  {
    public static float Apply(float value, TerrainDataAsset settings)
    {
      if (!settings.EnableHeightClamp)
        return value;

      value = Mathf.Clamp(value, settings.MinHeight, settings.MaxHeight);

      return value;
    }
  }
}