using UnityEngine;

namespace TerrainSystem.Runtime.Data
{
  [CreateAssetMenu(menuName = "Terrain/Terrain Data")]
  public class TerrainDataAsset : ScriptableObject
  {
    [Header("World")]
    public int ChunksX = 4;
    public int ChunksZ = 4;

    [Header("Chunk")]
    public int ChunkResolution = 32;
    public float ChunkSize = 20f;

    [Header("Noise")]
    public float NoiseScale = 0.03f;
    public float HeightMultiplier = 10f;
    public int Seed = 12345;

    [Header("Peak Control")]
    [Range(0f, 1f)]
    public float PeakFlattenStrength = 0f;

    public float PeakRadius = 10f;

    [Header("Rendering")]
    public Material TerrainMaterial;
    
    [Header("Shape")]
    public AnimationCurve HeightCurve = AnimationCurve.Linear(0, 0, 1, 1);
  }
}