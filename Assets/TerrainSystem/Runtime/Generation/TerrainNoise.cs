using TerrainSystem.Runtime.Data;
using UnityEngine;

namespace TerrainSystem.Runtime.Generation
{
  public static class TerrainNoise
  {
    public static float GetHeight(
      float x,
      float z,
      TerrainDataAsset data)
    {
      float height = 0f;

      float amplitude = 1f;

      float frequency =
        data.NoiseScale;

      float total = 0f;

      for (int i = 0; i < 4; i++)
      {
        float sample =
          Mathf.PerlinNoise(
            (x + data.Seed)
            * frequency,
            (z + data.Seed)
            * frequency
          );

        height +=
          sample * amplitude;

        total += amplitude;

        amplitude *= 0.5f;

        frequency *= 2f;
      }

      height /= total;

      height =
        data.HeightCurve
          .Evaluate(height);

      return
        height *
        data.HeightMultiplier;
    }
  }
}