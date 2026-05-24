using TerrainSystem.Runtime.Data;
using UnityEngine;

namespace TerrainSystem.Runtime.Rendering
{
  [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
  public class TerrainRenderer : MonoBehaviour
  {
    [SerializeField] private TerrainDataAsset _terrainData;

    private Mesh mesh;
    private TerrainMeshBuilder builder;

    public void SetData(TerrainDataAsset data)
    {
      _terrainData = data;
    }

    public void Generate()
    {
      if (_terrainData == null)
        return;

      builder = new TerrainMeshBuilder(_terrainData);
      mesh = builder.Build();

      var mf = GetComponent<MeshFilter>();
      var mr = GetComponent<MeshRenderer>();
      var mc = GetComponent<MeshCollider>();

      mf.sharedMesh = mesh;
      mc.sharedMesh = mesh;
      mr.sharedMaterial = _terrainData.TerrainMaterial;
    }
    public void ModifyHeight(Vector3 worldPos, float radius, float strength)
    {
      if (builder == null || builder.Vertices == null)
        return;

      Vector3[] v = builder.Vertices;

      for (int i = 0; i < v.Length; i++)
      {
        Vector2 p = new Vector2(v[i].x, v[i].z);
        Vector2 c = new Vector2(worldPos.x, worldPos.z);

        float dist = Vector2.Distance(p, c);

        if (dist > radius)
          continue;

        float falloff = 1f - dist / radius;

        v[i].y += strength * falloff;
      }

      mesh.vertices = v;
      mesh.RecalculateNormals();
      mesh.RecalculateBounds();

      GetComponent<MeshCollider>().sharedMesh = mesh;
    }
  }
}