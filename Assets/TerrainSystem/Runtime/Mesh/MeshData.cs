using Unity.Collections;
using UnityEngine;
using System;

namespace TerrainSystem.Runtime.Mesh
{
  public sealed class MeshData : IDisposable
  {
    public NativeArray<Vector3> Vertices;
    public NativeArray<int> Triangles;
    public NativeArray<Vector2> UVs;
    public NativeArray<Vector3> Normals;

    public void Dispose()
    {
      if (Vertices.IsCreated)
        Vertices.Dispose();

      if (Triangles.IsCreated)
        Triangles.Dispose();

      if (UVs.IsCreated)
        UVs.Dispose();

      if (Normals.IsCreated)
        Normals.Dispose();
    }
  }
}