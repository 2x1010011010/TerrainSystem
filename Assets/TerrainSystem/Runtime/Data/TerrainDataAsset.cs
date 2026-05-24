using UnityEngine;

namespace TerrainSystem.Runtime.Data
{
  [CreateAssetMenu(
    menuName = "Terrain/Terrain Data")]
  public class TerrainDataAsset :
    ScriptableObject
  {
    [Header("World")] 
    public int ChunksX = 4;
    public int ChunksZ = 4;

    [Header("Chunk")] 
    public int ChunkResolution = 32;
    public float ChunkSize = 16f;

    [Header("Noise")] 
    public float NoiseScale = 0.03f;
    public float HeightMultiplier = 10f;
    public int Seed = 12345;
    public AnimationCurve HeightCurve =
      AnimationCurve.EaseInOut(
        0,
        0,
        1,
        1
      );
    
    [Header("Rendering")]
    public Material TerrainMaterial;

    [HideInInspector] public TerrainWorldData WorldData =
      new();
  }
}