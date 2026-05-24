using TerrainSystem.Runtime.Rendering;
using UnityEngine;

namespace TerrainSystem.Runtime.Tools
{
  public class TerrainBrush : MonoBehaviour
  {
    public TerrainRenderer Terrain;
    public float Radius = 5f;
    public float Strength = 2f;

    private void Update()
    {
      if (!Input.GetMouseButton(0))
        return;

      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      if (Physics.Raycast(ray, out var hit))
      {
        Terrain.ModifyHeight(hit.point, Radius, Strength * Time.deltaTime);
      }
    }
  }
}