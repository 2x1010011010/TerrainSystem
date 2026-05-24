using System.Collections.Generic;
using UnityEngine;

namespace TerrainSystem
{
    /// <summary>
    /// Generates per-chunk meshes and combines them into a single mesh on the MeshFilter.
    /// All vertices are in the local space of the owning GameObject.
    /// The terrain is centred at the object's origin on the XZ plane.
    /// </summary>
    public static class TerrainMeshBuilder
    {
        public static Mesh Build(TerrainSettings s)
        {
            Vector2 totalSize = s.TotalSize;
            Vector2 origin = -totalSize * 0.5f; // centre the terrain at local origin

            var combineInstances = new List<CombineInstance>();

            for (int cy = 0; cy < s.chunkCount.y; cy++)
            {
                for (int cx = 0; cx < s.chunkCount.x; cx++)
                {
                    Vector2 chunkOffset = new Vector2(
                        origin.x + cx * s.chunkSize.x,
                        origin.y + cy * s.chunkSize.y
                    );

                    Mesh chunkMesh = BuildChunk(cx, cy, chunkOffset, s);

                    combineInstances.Add(new CombineInstance
                    {
                        mesh = chunkMesh,
                        transform = Matrix4x4.identity // already in local space
                    });
                }
            }

            Mesh combined = new Mesh
            {
                name = "TerrainMesh",
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 // support large meshes
            };
            combined.CombineMeshes(combineInstances.ToArray(), true, true);
            combined.RecalculateBounds();
            combined.RecalculateNormals();
            combined.RecalculateTangents();

            // Clean up temporary chunk meshes
            foreach (var ci in combineInstances)
                Object.DestroyImmediate(ci.mesh);

            return combined;
        }

        // ------------------------------------------------------------------ //

        static Mesh BuildChunk(int cx, int cy, Vector2 chunkOffset, TerrainSettings s)
        {
            int vertsX = s.chunkResolution.x + 1;
            int vertsZ = s.chunkResolution.y + 1;
            int vertCount = vertsX * vertsZ;

            var vertices  = new Vector3[vertCount];
            var uvs       = new Vector2[vertCount];
            var triangles = new int[s.chunkResolution.x * s.chunkResolution.y * 6];

            Vector2 cellSize = new Vector2(
                s.chunkSize.x / s.chunkResolution.x,
                s.chunkSize.y / s.chunkResolution.y
            );

            // Build vertices
            for (int z = 0; z < vertsZ; z++)
            {
                for (int x = 0; x < vertsX; x++)
                {
                    float wx = chunkOffset.x + x * cellSize.x;
                    float wz = chunkOffset.y + z * cellSize.y;
                    float h  = TerrainNoise.Sample(wx, wz, s) * s.heightScale * s.heightMultiplier;

                    int idx = z * vertsX + x;
                    vertices[idx] = new Vector3(wx, h, wz);
                    uvs[idx]      = new Vector2(
                        (wx - (-s.TotalSize.x * 0.5f)) / s.TotalSize.x,
                        (wz - (-s.TotalSize.y * 0.5f)) / s.TotalSize.y
                    );
                }
            }

            // Build triangles
            int triIdx = 0;
            for (int z = 0; z < s.chunkResolution.y; z++)
            {
                for (int x = 0; x < s.chunkResolution.x; x++)
                {
                    int tl = z       * vertsX + x;
                    int tr = z       * vertsX + x + 1;
                    int bl = (z + 1) * vertsX + x;
                    int br = (z + 1) * vertsX + x + 1;

                    // Triangle 1
                    triangles[triIdx++] = tl;
                    triangles[triIdx++] = bl;
                    triangles[triIdx++] = tr;
                    // Triangle 2
                    triangles[triIdx++] = tr;
                    triangles[triIdx++] = bl;
                    triangles[triIdx++] = br;
                }
            }

            var mesh = new Mesh { name = $"Chunk_{cx}_{cy}" };
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triangles, 0);
            return mesh;
        }
    }
}
