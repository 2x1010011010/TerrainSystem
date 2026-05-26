using TerrainSystem.Runtime.Data;
using TerrainSystem.Runtime.Streaming;
using UnityEngine;
using UnityEngine.Serialization;

namespace TerrainSystem.Runtime.Core
{
  [ExecuteAlways]
  public sealed class TerrainGenerator : MonoBehaviour
  {
    [SerializeField] private TerrainDataAsset _terrainData;
    [SerializeField] private Transform _target;

    private TerrainStreamer _streamer;

    public TerrainDataAsset TerrainData => _terrainData;

    private void OnEnable() => 
      Initialize();

    private void Update() => 
      _streamer?.Tick();

    private void OnDisable() => 
      _streamer?.Dispose();

    public void Regenerate() => 
      _streamer?.Regenerate();

    private void Initialize()
    {
      if (_terrainData == null)
      {
        Debug.LogError("TerrainDataAsset missing.");
        return;
      }

      _streamer = new TerrainStreamer(transform, _target, _terrainData);
    }
  }
}