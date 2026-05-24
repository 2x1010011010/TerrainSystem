using System.Collections.Generic;
using TerrainSystem.Runtime.Data;
using TerrainSystem.Runtime.Generation;
using UnityEngine;

namespace TerrainSystem.Runtime.Rendering
{
  public class TerrainMeshBuilder
  {
    private readonly TerrainDataAsset data;
    public Vector3[] Vertices;
    public int[] Triangles;
    public Vector2[] UVs;

    public TerrainMeshBuilder(TerrainDataAsset data)
    {
      this.data = data;
    }

    public Mesh Build()
    {
      int resolution = data.ChunkResolution;
      int worldResolutionX = data.ChunksX * resolution;
      int worldResolutionZ = data.ChunksZ * resolution;
      int vertsX = worldResolutionX + 1;
      int vertsZ = worldResolutionZ + 1;

      Vertices = new Vector3[vertsX * vertsZ];

      UVs = new Vector2[vertsX * vertsZ];

      List<int> tris = new();

      GenerateVertices(worldResolutionX, worldResolutionZ);
      GenerateTriangles(worldResolutionX, worldResolutionZ, tris);
      Triangles = tris.ToArray();

      Mesh mesh = new Mesh();

      mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
      mesh.vertices = Vertices;
      mesh.triangles = Triangles;
      mesh.uv = UVs;
      mesh.RecalculateNormals();
      mesh.RecalculateBounds();

      return mesh;
    }

    private void GenerateVertices(int worldX, int worldZ)
    {
      float step = data.ChunkSize / data.ChunkResolution;

      for (int z = 0; z <= worldZ; z++)
      {
        for (int x = 0; x <= worldX; x++)
        {
          int i = z * (worldX + 1) + x;
          float wx = x * step;
          float wz = z * step;
          float h = TerrainNoise.GetHeight(wx,wz, data);

          Vertices[i] = new Vector3(wx, h, wz);

          UVs[i] = new Vector2((float)x / worldX, (float)z / worldZ);
        }
      }
    }

    private void GenerateTriangles(int worldX, int worldZ, List<int> tris)
    {
      for (int z = 0; z < worldZ; z++)
      {
        for (int x = 0; x < worldX; x++)
        {
          int i = z * (worldX + 1) + x;
          tris.Add(i);
          tris.Add(i + worldX + 1);
          tris.Add(i + 1);
          tris.Add(i + 1);
          tris.Add(i + worldX + 1);
          tris.Add(i + worldX + 2);
        }
      }
    }
  }
}