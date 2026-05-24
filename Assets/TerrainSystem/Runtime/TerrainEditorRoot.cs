using TerrainSystem.Runtime.Data;
using TerrainSystem.Runtime.Rendering;
using UnityEngine;

namespace TerrainSystem.Runtime
{
  [ExecuteAlways]
  public class TerrainEditorRoot : MonoBehaviour
  {
    public TerrainDataAsset TerrainData;

    private TerrainRenderer renderer;

    private void OnEnable()
    {
      AutoSetup();

      renderer = GetComponent<TerrainRenderer>();

      if (renderer == null)
        renderer = gameObject.AddComponent<TerrainRenderer>();

      renderer.SetData(TerrainData);
      renderer.Generate();
    }

    private void AutoSetup()
    {
      if (TerrainData == null)
      {
        TerrainData = TerrainAutoBootstrap.GetOrCreateData();
      }

      if (TerrainData.TerrainMaterial == null)
      {
        TerrainData.TerrainMaterial =
          TerrainAutoBootstrap.GetOrCreateMaterial();
      }
    }

    public void Regenerate()
    {
      renderer.Generate();
    }
  }
}