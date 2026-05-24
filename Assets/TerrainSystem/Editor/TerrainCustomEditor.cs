using UnityEditor;
using UnityEngine;
using TerrainSystem.Runtime;

namespace TerrainSystem.Editor
{
  [CustomEditor(typeof(TerrainEditorRoot))]
  public class TerrainCustomEditor : UnityEditor.Editor
  {
    private TerrainEditorRoot root;
    private int tab;

    private void OnEnable()
    {
      root = (TerrainEditorRoot)target;

      SceneView.duringSceneGui += OnSceneGUI;

      Tools.hidden = true;
    }

    private void OnDisable()
    {
      SceneView.duringSceneGui -= OnSceneGUI;

      Tools.hidden = false;
    }

    public override void OnInspectorGUI()
    {
      GUILayout.Label("Terrain Editor", EditorStyles.boldLabel);

      root.TerrainData.ChunksX = EditorGUILayout.IntField("Chunks X", root.TerrainData.ChunksX);
      root.TerrainData.ChunksZ = EditorGUILayout.IntField("Chunks Z", root.TerrainData.ChunksZ);
      root.TerrainData.HeightMultiplier = EditorGUILayout.FloatField("Height", root.TerrainData.HeightMultiplier);
      root.TerrainData.NoiseScale = EditorGUILayout.Slider("Noise", root.TerrainData.NoiseScale, 0.001f, 1f);

      if (GUILayout.Button("Regenerate"))
      {
        root.Regenerate();
        EditorUtility.SetDirty(root.TerrainData);
      }
    }

    private void OnSceneGUI(SceneView view)
    {
      if (root == null) return;

      Vector3 center =
        new Vector3(
          root.TerrainData.ChunksX * root.TerrainData.ChunkSize * 0.5f,
          root.TerrainData.HeightMultiplier * 0.5f,
          root.TerrainData.ChunksZ * root.TerrainData.ChunkSize * 0.5f
        );

      EditorGUI.BeginChangeCheck();

      Vector3 scale =
        Handles.ScaleHandle(
          new Vector3(
            root.TerrainData.ChunksX,
            root.TerrainData.HeightMultiplier,
            root.TerrainData.ChunksZ
          ),
          center,
          Quaternion.identity,
          HandleUtility.GetHandleSize(center)
        );

      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(root.TerrainData, "Resize Terrain");

        root.TerrainData.ChunksX = Mathf.Max(1, Mathf.RoundToInt(scale.x));
        root.TerrainData.ChunksZ = Mathf.Max(1, Mathf.RoundToInt(scale.z));
        root.TerrainData.HeightMultiplier = Mathf.Max(1f, scale.y);

        root.Regenerate();

        EditorUtility.SetDirty(root.TerrainData);
      }
    }
  }
}