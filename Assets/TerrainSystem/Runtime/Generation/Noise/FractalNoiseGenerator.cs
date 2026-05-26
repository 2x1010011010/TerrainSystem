using TerrainSystem.Runtime.Data;
using UnityEngine;

namespace TerrainSystem.Runtime.Generation.Noise
{
  public sealed class FractalNoiseGenerator : INoiseGenerator
  {
    private readonly NoiseSettingsAsset _settings;

    public FractalNoiseGenerator(NoiseSettingsAsset settings) =>
      _settings = settings;

    public float Sample(float x, float z)
    {
      float amplitude = 1f;
      float frequency = 1f;
      float noiseHeight = 0f;
      float maxPossibleHeight = 0f;

      for (int octave = 0; octave < _settings.Octaves; octave++)
      {
        float sampleX = (x + _settings.Offset.x + _settings.Seed) / _settings.Scale * frequency;
        float sampleZ = (z + _settings.Offset.y + _settings.Seed) / _settings.Scale * frequency;
        float perlin = Mathf.PerlinNoise(sampleX, sampleZ) * 2f - 1f;
        noiseHeight += perlin * amplitude;
        maxPossibleHeight += amplitude;
        amplitude *= _settings.Persistence;
        frequency *= _settings.Lacunarity;
      }

      if (maxPossibleHeight > 0f)
        noiseHeight /= maxPossibleHeight;

      float normalized = Mathf.InverseLerp(-1f, 1f, noiseHeight);
      normalized = _settings.HeightCurve.Evaluate(normalized);

      return normalized * _settings.HeightMultiplier;
    }
  }
}