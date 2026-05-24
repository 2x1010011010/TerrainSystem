using UnityEngine;
using TerrainSystem.Runtime.Data;

namespace TerrainSystem.Runtime.Rendering
{
  public class TerrainMeshBuilder
  {
    private TerrainDataAsset data;
    public Vector3[] Vertices { get; private set; }

    private int resolution;
    private float step;

    public TerrainMeshBuilder(TerrainDataAsset data)
    {
      this.data = data;
    }

    public Mesh Build()
    {
      Mesh mesh = new Mesh();
      mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

      resolution = data.ChunksX * data.ChunkResolution;
      step = data.ChunkSize / data.ChunkResolution;

      Vertices = new Vector3[(resolution + 1) * (resolution + 1)];
      int[] triangles = new int[resolution * resolution * 6];

      for (int z = 0; z <= resolution; z++)
      {
        for (int x = 0; x <= resolution; x++)
        {
          float worldX = x * step;
          float worldZ = z * step;

          float height =
            Mathf.PerlinNoise(worldX * data.NoiseScale,
              worldZ * data.NoiseScale)
            * data.HeightMultiplier;

          int i = z * (resolution + 1) + x;

          Vertices[i] = new Vector3(worldX, height, worldZ);
        }
      }

      int t = 0;

      for (int z = 0; z < resolution; z++)
      {
        for (int x = 0; x < resolution; x++)
        {
          int i = z * (resolution + 1) + x;

          triangles[t++] = i;
          triangles[t++] = i + resolution + 1;
          triangles[t++] = i + 1;

          triangles[t++] = i + 1;
          triangles[t++] = i + resolution + 1;
          triangles[t++] = i + resolution + 2;
        }
      }

      mesh.vertices = Vertices;
      mesh.triangles = triangles;

      mesh.RecalculateNormals();
      mesh.RecalculateBounds();

      return mesh;
    }
  }
}