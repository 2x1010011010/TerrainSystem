using TerrainSystem.Runtime.Data;
using TerrainSystem.Runtime.Generation.Noise;

namespace TerrainSystem.Runtime.Generation.Height
{
  public sealed class TerrainHeightProvider : IHeightProvider
  {
    private readonly INoiseGenerator _noise;
    private readonly TerrainDataAsset _data;

    public TerrainHeightProvider(INoiseGenerator noise, TerrainDataAsset data)
    {
      _noise = noise;
      _data = data;
    }

    public float GetHeight(float x, float z)
    {
      float value = _noise.Sample(x, z);

      value = HeightClipper.Apply(value, _data);

      return value;
    }
  }
}