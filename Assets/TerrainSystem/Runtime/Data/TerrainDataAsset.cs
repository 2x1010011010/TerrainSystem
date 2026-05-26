using UnityEngine;
using UnityEngine.Serialization;

namespace TerrainSystem.Runtime.Data
{
  [CreateAssetMenu(
    fileName = "TerrainData",
    menuName = "TerrainSystem/Terrain Data")]
  public sealed class TerrainDataAsset : ScriptableObject
  {

    [Header("Chunk Settings")]
    [SerializeField]
    private int _chunkResolution = 128;
    [SerializeField]
    private float _chunkSize = 128f;
    [SerializeField]
    private int _visibleRadius = 4;
    
    [Header("Terrain Dimensions")]
    [SerializeField]
    private float _terrainHeight = 40f;
    [SerializeField]
    private int _chunkCountX = 4;
    [SerializeField]
    private int _chunkCountZ = 4;

    [Header("Noise")]
    [SerializeField]
    private NoiseSettingsAsset _noiseSettings;

    [Header("Material")]
    [SerializeField]
    private Material _terrainMaterial;

    [Header("Clipping")]
    [SerializeField]
    private bool _enableHeightClamp;
    [SerializeField]
    private float _minHeight;
    [SerializeField]
    private float _maxHeight = 100f;

    public int ChunkResolution => _chunkResolution;
    public float ChunkSize => _chunkSize;
    public int VisibleRadius => _visibleRadius;
    public float TerrainHeight => _terrainHeight;
    public int ChunkCountX => _chunkCountX;
    public int ChunkCountZ => _chunkCountZ;
    public NoiseSettingsAsset NoiseSettings => _noiseSettings;
    public Material TerrainMaterial => _terrainMaterial;
    public bool EnableHeightClamp => _enableHeightClamp;
    public float MinHeight => _minHeight;
    public float MaxHeight => _maxHeight;
  }
}