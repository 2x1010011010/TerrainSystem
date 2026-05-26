using UnityEngine;

namespace TerrainSystem.Runtime.Data
{
  [CreateAssetMenu(
    fileName = "TerrainMaterialSettings",
    menuName = "TerrainSystem/Terrain Material Settings")]
  public sealed class TerrainMaterialSettings : ScriptableObject
  {
    [Header("Material")] 
    [SerializeField] private Material _terrainMaterial;

    [Header("Tiling")] 
    [SerializeField] private Vector2 _tiling = Vector2.one;
    [SerializeField] private Vector2 _offset = Vector2.zero;

    [Header("Height Blending")] 
    [SerializeField] private bool _enableHeightBlending = true;
    [SerializeField] private float _blendStrength = 4f;

    [Header("Normal Mapping")] 
    [SerializeField] private bool _enableNormalMap = true;
    [SerializeField] private float _normalStrength = 1f;
    
    public Material TerrainMaterial => _terrainMaterial;
    public Vector2 Tiling => _tiling;
    public Vector2 Offset => _offset;
    public bool EnableHeightBlending => _enableHeightBlending;
    public float BlendStrength => _blendStrength;
    public bool EnableNormalMap => _enableNormalMap;
    public float NormalStrength => _normalStrength;

    public void Apply()
    {
      if (_terrainMaterial == null)
      {
        Debug.LogWarning("[TerrainSystem] Terrain material is null.");
        return;
      }

      _terrainMaterial.mainTextureOffset = _offset;
      _terrainMaterial.mainTextureScale = _tiling;
      _terrainMaterial.SetFloat("_BlendStrength", _blendStrength);
      _terrainMaterial.SetFloat("_NormalStrength", _normalStrength);
    }
  }
}