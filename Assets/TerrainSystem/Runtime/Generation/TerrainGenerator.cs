using UnityEngine;

namespace TerrainSystem
{
    /// <summary>
    /// Attach to any GameObject. Drives mesh generation from a TerrainSettings asset.
    /// Call Generate() to (re)build the terrain mesh.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    [DisallowMultipleComponent]
    public class TerrainGenerator : MonoBehaviour
    {
        [Tooltip("Shared settings asset. Create via Assets > Terrain System > Terrain Settings.")]
        public TerrainSettings settings;

        // ---- internal state accessible from editor tools -------------------

        [HideInInspector] public bool autoRegenerate = false;

        MeshFilter   _meshFilter;
        MeshRenderer _meshRenderer;
        Mesh         _mesh;

        // ------------------------------------------------------------------ //
        //  Public API
        // ------------------------------------------------------------------ //

        /// <summary>Rebuild the terrain mesh from the current settings.</summary>
        public void Generate()
        {
            if (settings == null)
            {
                Debug.LogWarning("[TerrainGenerator] No TerrainSettings assigned.", this);
                return;
            }

            EnsureComponents();

            // Destroy old mesh to avoid leaks in the editor
            if (_mesh != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(_mesh);
#else
                Destroy(_mesh);
#endif
            }

            _mesh = TerrainMeshBuilder.Build(settings);
            _meshFilter.sharedMesh = _mesh;

            if (settings.terrainMaterial != null)
                _meshRenderer.sharedMaterial = settings.terrainMaterial;
        }

        /// <summary>
        /// Resize the terrain in world-space units.
        /// X / Z change the footprint; Y changes the height multiplier only.
        /// </summary>
        public void ApplySize(Vector3 newSize)
        {
            if (settings == null) return;

            // Clamp to sensible minimums
            newSize.x = Mathf.Max(newSize.x, 0.1f);
            newSize.y = Mathf.Max(newSize.y, 0.01f);
            newSize.z = Mathf.Max(newSize.z, 0.1f);

            // Derive chunk sizes so total footprint equals (newSize.x, newSize.z)
            settings.chunkSize = new Vector2(
                newSize.x / settings.chunkCount.x,
                newSize.z / settings.chunkCount.y
            );

            // Y drives the height multiplier; keep heightScale as base reference
            settings.heightMultiplier = newSize.y;

            Generate();
        }

        /// <summary>Returns the current logical size (X, heightMultiplier, Z).</summary>
        public Vector3 GetSize()
        {
            if (settings == null) return Vector3.one;
            return new Vector3(
                settings.TotalSize.x,
                settings.heightMultiplier,
                settings.TotalSize.y
            );
        }

        // ------------------------------------------------------------------ //
        //  Unity lifecycle
        // ------------------------------------------------------------------ //

        void Awake()
        {
            EnsureComponents();
            if (_meshFilter.sharedMesh == null && settings != null)
                Generate();
        }

        void EnsureComponents()
        {
            if (_meshFilter   == null) _meshFilter   = GetComponent<MeshFilter>();
            if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (autoRegenerate && settings != null)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (this != null) Generate();
                };
            }
        }
#endif
    }
}
