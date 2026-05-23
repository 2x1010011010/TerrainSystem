namespace TerrainSystem.Runtime.Data
{
  public static class TerrainMapUtility
  {
    public static int ToIndex(int x, int z, int resolution)
    {
      return z * (resolution + 1) + x;
    }
  }
}