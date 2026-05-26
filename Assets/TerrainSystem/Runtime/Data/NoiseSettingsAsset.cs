using AnimationCurve = UnityEngine.AnimationCurve;
using UnityEngine;

namespace TerrainSystem.Runtime.Data
{
  [CreateAssetMenu(
    fileName = "NoiseSettings",
    menuName = "TerrainSystem/Noise Settings")]
  public sealed class NoiseSettingsAsset : ScriptableObject
  {
    [SerializeField]
    private int seed = 1337;

    [SerializeField]
    private float scale = 100f;

    [SerializeField]
    private int octaves = 4;

    [SerializeField]
    private float persistence = 0.5f;

    [SerializeField]
    private float lacunarity = 2f;

    [SerializeField]
    private float heightMultiplier = 25f;

    [SerializeField]
    private Vector2 offset;

    [SerializeField]
    private AnimationCurve heightCurve =
      AnimationCurve.Linear(0f, 0f, 1f, 1f);

    public int Seed => seed;
    public float Scale => scale;
    public int Octaves => octaves;
    public float Persistence => persistence;
    public float Lacunarity => lacunarity;
    public float HeightMultiplier => heightMultiplier;
    public Vector2 Offset => offset;
    public AnimationCurve HeightCurve => heightCurve;
  }
}