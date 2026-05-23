using UnityEngine;

namespace TerrainSystem.Runtime.Generation
{
  public static class TerrainNoiseGenerator
  {
    public static float GenerateHeight(float worldX, float worldZ, float scale, float maxHeight, int seed, AnimationCurve terrainCurve)
    {
      float height = 0f;

      float frequency = scale;
      float amplitude = 1f;

      float totalAmplitude = 0f;

      // LARGE SHAPES
      for (int octave = 0; octave < 4; octave++)
      {
        float sampleX =
          (worldX + seed) * frequency;

        float sampleZ =
          (worldZ + seed) * frequency;

        float noise =
          Mathf.PerlinNoise(
            sampleX,
            sampleZ
          );

        height += noise * amplitude;

        totalAmplitude += amplitude;

        amplitude *= 0.5f;
        frequency *= 2f;
      }

      height /= totalAmplitude;
      height = Mathf.Clamp01(height);

      // Smooth terrain curve
      height = terrainCurve.Evaluate(height);

      return height * maxHeight;
    }
  }
}