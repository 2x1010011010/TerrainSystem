using TerrainSystem.Runtime.Data;
using UnityEngine;

namespace TerrainSystem.Runtime.Generation.Noise
{
  public sealed class PerlinNoiseGenerator : INoiseGenerator
  {
    private readonly NoiseSettingsAsset _settings;

    public PerlinNoiseGenerator(NoiseSettingsAsset settings) => 
      _settings = settings;

    public float Sample(float x, float z)
    {
      float amplitude = 1f;
      float frequency = 1f;
      float noiseHeight = 0f;

      for (int i = 0; i < _settings.Octaves; i++)
      {
        float sampleX = (x + _settings.Offset.x + _settings.Seed) / _settings.Scale * frequency;
        float sampleZ = (z + _settings.Offset.y + _settings.Seed) / _settings.Scale * frequency;
        float perlin = Mathf.PerlinNoise(sampleX, sampleZ) * 2f - 1f;

        noiseHeight += perlin * amplitude;
        amplitude *= _settings.Persistence;
        frequency *= _settings.Lacunarity;
      }

      float normalized = Mathf.InverseLerp(-1f, 1f, noiseHeight);

      return _settings.HeightCurve.Evaluate(normalized) * _settings.HeightMultiplier;
    }
  }
}