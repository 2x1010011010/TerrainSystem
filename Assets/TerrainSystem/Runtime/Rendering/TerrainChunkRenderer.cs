using TerrainSystem.Runtime.Data;
using UnityEngine;

namespace TerrainSystem.Runtime.Rendering
{
  [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
  public class TerrainChunkRenderer : MonoBehaviour
  {
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    public void Build(ChunkData chunkData, TerrainDataAsset terrainData)
    {
      meshFilter = GetComponent<MeshFilter>();
      meshRenderer = GetComponent<MeshRenderer>();
      meshCollider = GetComponent<MeshCollider>();

      Mesh mesh = GenerateMesh(chunkData, terrainData);

      meshFilter.sharedMesh = mesh;
      meshCollider.sharedMesh = mesh;

      meshRenderer.sharedMaterial =
        terrainData.TerrainMaterial;
    }

    private Mesh GenerateMesh(ChunkData chunkData, TerrainDataAsset terrainData)
    {
      Mesh mesh = new Mesh();

      mesh.indexFormat =
        UnityEngine.Rendering.IndexFormat.UInt32;

      int resolution = terrainData.ChunkResolution;
      float size = terrainData.ChunkSize;

      float step = size / resolution;

      Vector3[] vertices =
        new Vector3[(resolution + 1) * (resolution + 1)];

      Vector2[] uvs =
        new Vector2[vertices.Length];

      int[] triangles =
        new int[resolution * resolution * 6];

      for (int z = 0; z <= resolution; z++)
      {
        for (int x = 0; x <= resolution; x++)
        {
          int index =
            TerrainMapUtility.ToIndex(
              x,
              z,
              resolution
            );

          float height =
            chunkData.Heights[index];

          vertices[index] = new Vector3(
            x * step,
            height,
            z * step
          );

          uvs[index] = new Vector2(
            x / (float)resolution,
            z / (float)resolution
          );
        }
      }

      int triIndex = 0;

      for (int z = 0; z < resolution; z++)
      {
        for (int x = 0; x < resolution; x++)
        {
          int i =
            TerrainMapUtility.ToIndex(
              x,
              z,
              resolution
            );

          triangles[triIndex++] = i;
          triangles[triIndex++] = i + resolution + 1;
          triangles[triIndex++] = i + 1;

          triangles[triIndex++] = i + 1;
          triangles[triIndex++] = i + resolution + 1;
          triangles[triIndex++] = i + resolution + 2;
        }
      }

      mesh.vertices = vertices;
      mesh.uv = uvs;
      mesh.triangles = triangles;

      mesh.RecalculateNormals();
      mesh.RecalculateBounds();

      return mesh;
    }
  }
}