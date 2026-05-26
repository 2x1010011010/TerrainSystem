using TerrainSystem.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace TerrainSystem.Editor.Inspectors
{
  [CustomEditor(typeof(TerrainGenerator))]
  public sealed class TerrainGeneratorEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      GUILayout.Space(10f);

      TerrainGenerator generator = (TerrainGenerator)target;

      if (GUILayout.Button("Generate"))
        generator.Regenerate();
      
      if (GUILayout.Button("Clear"))
        generator.SendMessage("OnDisable", SendMessageOptions.DontRequireReceiver);
    }
  }
}