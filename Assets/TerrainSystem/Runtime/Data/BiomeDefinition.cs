using UnityEngine;

namespace TerrainSystem.Runtime.Data
{
  [CreateAssetMenu(menuName = "Terrain System/Biome")]
  public class BiomeDefinition : ScriptableObject
  {
    [Header("Vegetation")]
    public GameObject[] Trees;
    public GameObject[] Bushes;
    public GameObject[] Grass;

    [Header("Placement")]
    public float Density = 1f;

    [Header("Height")]
    public float MinHeight = 0;
    public float MaxHeight = 10;

    [Header("Slope")]
    public float MaxSlope = 30f;
  }
}