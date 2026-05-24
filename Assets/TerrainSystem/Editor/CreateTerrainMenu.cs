using TerrainSystem.Runtime;
using UnityEditor;
using UnityEngine;

namespace TerrainSystem.Editor
{
  public static class CreateTerrainMenu
  {
    [MenuItem("Tools/Terrain System/Create Terrain")]
    public static void Create()
    {
      var go = new GameObject("Terrain");

      var root = go.AddComponent<TerrainEditorRoot>();

      Selection.activeGameObject = go;

      Debug.Log("Terrain created (auto system will initialize everything).");
    }
  }
}