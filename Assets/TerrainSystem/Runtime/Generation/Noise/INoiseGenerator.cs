namespace TerrainSystem.Runtime.Generation.Noise
{
  public interface INoiseGenerator
  {
    float Sample(float x, float z);
  }
}