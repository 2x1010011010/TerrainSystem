using TerrainSystem.Runtime.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace TerrainSystem.Runtime.Rendering
{
  [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
  public class TerrainRenderer : MonoBehaviour
  {
    [SerializeField] private TerrainDataAsset _terrainData;

    private Mesh mesh;
    private TerrainMeshBuilder builder;
    public TerrainDataAsset Data => _terrainData;
    
    public void SetData(TerrainDataAsset data)
    {
      _terrainData = data;
    }

    public void Generate()
    {
      builder = new TerrainMeshBuilder(_terrainData);
      mesh = builder.Build();
      GetComponent<MeshFilter>().sharedMesh = mesh;
      GetComponent<MeshCollider>().sharedMesh = mesh;
      GetComponent<MeshRenderer>().sharedMaterial = _terrainData.TerrainMaterial;
    }

    public void UpdateTerrain()
    {
      mesh.vertices = builder.Vertices;
      mesh.RecalculateNormals();
      mesh.RecalculateBounds();
      mesh.UploadMeshData(false);
      GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public void ModifyHeight(Vector3 worldPos, float radius, float strength)
    {
      for (int i = 0; i < builder.Vertices.Length; i++)
      {
        Vector3 v = builder.Vertices[i];
        Vector2 flat = new Vector2(v.x, v.z);
        Vector2 center = new Vector2(worldPos.x, worldPos.z);

        float dist = Vector2.Distance(flat, center);

        if (dist > radius)
          continue;

        float falloff = 1f - dist / radius;

        v.y += strength * falloff;

        builder.Vertices[i] = v;
      }

      UpdateTerrain();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
      if (!Application.isPlaying && _terrainData != null)
        Generate();
    }
#endif
  }
}