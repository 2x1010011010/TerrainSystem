using TerrainSystem.Runtime.Data;
using TerrainSystem.Runtime.Generation.Height;
using UnityEngine;

namespace TerrainSystem.Runtime.Mesh
{
  public static class MeshGenerator
  {
    public static UnityEngine.Mesh Generate(Vector2Int chunkCoordinate, TerrainDataAsset data, IHeightProvider heightProvider)
    {
      int resolution = data.ChunkResolution;
      float size = data.ChunkSize;
      float step = size / (resolution - 1);

      Vector3[] vertices = new Vector3[resolution * resolution];
      Vector2[] uvs = new Vector2[vertices.Length];

      int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
      int vertexIndex = 0;

      for (int z = 0; z < resolution; z++)
      {
        for (int x = 0; x < resolution; x++)
        {
          float worldX = chunkCoordinate.x * size + x * step;
          float worldZ = chunkCoordinate.y * size + z * step;
          float height = heightProvider.GetHeight(worldX, worldZ);

          vertices[vertexIndex] = new Vector3(x * step, height, z * step);
          uvs[vertexIndex] = new Vector2((float)x / resolution, (float)z / resolution);

          vertexIndex++;
        }
      }

      int triangleIndex = 0;

      for (int z = 0; z < resolution - 1; z++)
      {
        for (int x = 0; x < resolution - 1; x++)
        {
          int current =
            z * resolution + x;

          triangles[triangleIndex++] = current;
          triangles[triangleIndex++] = current + resolution + 1;
          triangles[triangleIndex++] = current + resolution;

          triangles[triangleIndex++] = current;
          triangles[triangleIndex++] = current + 1;
          triangles[triangleIndex++] = current + resolution + 1;
        }
      }

      UnityEngine.Mesh mesh = new UnityEngine.Mesh{indexFormat = UnityEngine.Rendering.IndexFormat.UInt32};

      mesh.vertices = vertices;
      mesh.triangles = triangles;
      mesh.uv = uvs;

      mesh.RecalculateNormals();
      mesh.RecalculateBounds();

      return mesh;
    }
  }
}