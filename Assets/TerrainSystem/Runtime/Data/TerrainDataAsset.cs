using System.Collections.Generic;
using UnityEngine;

namespace TerrainSystem.Runtime.Data
{
  [CreateAssetMenu(menuName = "Terrain System/Terrain Data")]
  public class TerrainDataAsset : ScriptableObject
  {
    [Header("World Size")]
    public int ChunksX = 4;
    public int ChunksZ = 4;

    [Header("Chunk Settings")]
    public int ChunkResolution = 32;
    public float ChunkSize = 20f;

    [Header("Height")]
    public float HeightMultiplier = 10f;

    [Header("Chunks")]
    public List<ChunkData> Chunks = new();
    
    [Header("Height Curve")]
    public AnimationCurve HeightCurve =
      AnimationCurve.EaseInOut(
        0,
        0,
        1,
        1
      );

    public Material TerrainMaterial { get; set; }
    public int Seed { get; set; }
    public float NoiseScale { get; set; }
  }
}