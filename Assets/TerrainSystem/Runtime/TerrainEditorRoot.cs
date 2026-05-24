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

    private bool dirty;
    private float lastUpdate;

    private const float updateDelay = 0.1f;

    private void OnEnable()
    {
      AutoSetup();

      renderer = GetComponent<TerrainRenderer>();
      if (renderer == null)
        renderer = gameObject.AddComponent<TerrainRenderer>();

      renderer.SetData(TerrainData);
      renderer.Generate();
    }

    private void Update()
    {
      if (!dirty) return;

      if (Time.realtimeSinceStartup - lastUpdate < updateDelay)
        return;

      lastUpdate = Time.realtimeSinceStartup;
      dirty = false;

      renderer.SetData(TerrainData);
      renderer.Generate();
    }

    public void MarkDirty()
    {
      dirty = true;
    }

    private void AutoSetup()
    {
      if (TerrainData == null)
        TerrainData = TerrainAutoBootstrap.GetOrCreateData();

      if (TerrainData.TerrainMaterial == null)
        TerrainData.TerrainMaterial = TerrainAutoBootstrap.GetOrCreateMaterial();
    }

    public void Regenerate()
    {
      MarkDirty();
    }
  }
}