using UnityEngine;
using TerrainSystem.Runtime.Data;

namespace TerrainSystem.Runtime.Rendering
{
  public class TerrainMeshBuilder
  {
    private TerrainDataAsset data;

    public Vector3[] Vertices { get; private set; }

    public TerrainMeshBuilder(TerrainDataAsset data)
    {
      this.data = data;
    }

    public Mesh Build()
    {
      Mesh mesh = new Mesh();
      mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

      int res = data.ChunksX * data.ChunkResolution;
      float step = data.ChunkSize / data.ChunkResolution;

      Vertices = new Vector3[(res + 1) * (res + 1)];
      int[] triangles = new int[res * res * 6];

      for (int z = 0; z <= res; z++)
      for (int x = 0; x <= res; x++)
      {
        float worldX = x * step;
        float worldZ = z * step;

        float height =
          Mathf.PerlinNoise(worldX * data.NoiseScale,
                             worldZ * data.NoiseScale)
          * data.HeightMultiplier;

        height = ApplyPeakFlatten(height, new Vector2(worldX, worldZ));

        int i = z * (res + 1) + x;
        Vertices[i] = new Vector3(worldX, height, worldZ);
      }

      int t = 0;

      for (int z = 0; z < res; z++)
      for (int x = 0; x < res; x++)
      {
        int i = z * (res + 1) + x;

        triangles[t++] = i;
        triangles[t++] = i + res + 1;
        triangles[t++] = i + 1;

        triangles[t++] = i + 1;
        triangles[t++] = i + res + 1;
        triangles[t++] = i + res + 2;
      }

      mesh.vertices = Vertices;
      mesh.triangles = triangles;

      mesh.RecalculateNormals();
      mesh.RecalculateBounds();

      return mesh;
    }

    private float ApplyPeakFlatten(float height, Vector2 worldPos)
    {
      if (data.PeakFlattenStrength <= 0f)
        return height;

      Vector2 center =
        new Vector2(
          data.ChunksX * data.ChunkSize * 0.5f,
          data.ChunksZ * data.ChunkSize * 0.5f
        );

      float dist = Vector2.Distance(worldPos, center);

      float influence =
        Mathf.Clamp01(1f - dist / data.PeakRadius);

      return height * (1f - influence * data.PeakFlattenStrength);
    }
  }
}