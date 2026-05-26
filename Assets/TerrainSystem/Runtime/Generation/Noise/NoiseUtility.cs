using Unity.Mathematics;
using UnityEngine;

namespace TerrainSystem.Runtime.Generation.Noise
{
  public static class NoiseUtility
  {
    public static float SamplePerlin(float x, float z) =>
      Mathf.PerlinNoise(x, z) * 2f - 1f;

    public static Vector2[] GenerateOctaveOffsets(int seed, int octaves, Vector2 globalOffset)
    {
      Vector2[] offsets = new Vector2[octaves];

      Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)(seed + 1));

      for (int i = 0; i < octaves; i++)
      {
        float offsetX = random.NextFloat(-100000f, 100000f) + globalOffset.x;
        float offsetZ = random.NextFloat(-100000f, 100000f) + globalOffset.y;

        offsets[i] = new Vector2(offsetX, offsetZ);
      }

      return offsets;
    }

    public static Vector2 DomainWarp(float x, float z, float strength, float frequency)
    {
      float warpX = SamplePerlin(x * frequency, z * frequency) * strength;
      float warpZ = SamplePerlin((x + 1000f) * frequency, (z + 1000f) * frequency) * strength;

      return new Vector2(x + warpX, z + warpZ);
    }

    public static float Remap(float value, float inputMin, float inputMax, float outputMin, float outputMax) =>
      outputMin + (value - inputMin) * (outputMax - outputMin) / (inputMax - inputMin);

    public static float SmoothFalloff(float value, float power)
    {
      value = Mathf.Clamp01(value);
      return Mathf.Pow(value, power) / (Mathf.Pow(value, power) + Mathf.Pow(1f - value, power));
    }

    public static float RadialFalloff(float x, float z, float radius)
    {
      float distance = Mathf.Sqrt(x * x + z * z);

      return Mathf.Clamp01(distance / radius);
    }
  }
}