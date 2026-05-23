using TerrainSystem.Runtime.Data;
using TerrainSystem.Runtime.Rendering;
using UnityEditor;
using UnityEngine;

namespace TerrainSystem.Editor
{
  public static class TerrainSizeTool
  {
    private static TerrainWorldRenderer renderer;
    private static TerrainDataAsset terrainData;

    public static void DrawGUI()
    {
      FindTerrain();

      if (renderer == null)
      {
        EditorGUILayout.HelpBox(
          "No terrain found.",
          MessageType.Warning);

        return;
      }

      EditorGUI.BeginChangeCheck();

      GUILayout.Label(
        "Terrain Size",
        EditorStyles.boldLabel);

      terrainData.ChunksX =
        EditorGUILayout.IntField(
          "Chunks X",
          terrainData.ChunksX);

      terrainData.ChunksZ =
        EditorGUILayout.IntField(
          "Chunks Z",
          terrainData.ChunksZ);

      GUILayout.Space(10);

      GUILayout.Label(
        "Terrain Height",
        EditorStyles.boldLabel);

      terrainData.HeightMultiplier =
        EditorGUILayout.FloatField(
          "Height",
          terrainData.HeightMultiplier);

      GUILayout.Space(10);

      GUILayout.Label(
        "Terrain Noise",
        EditorStyles.boldLabel);

      terrainData.NoiseScale =
        EditorGUILayout.Slider(
          "Noise Scale",
          terrainData.NoiseScale,
          0.001f,
          1f);

      if (EditorGUI.EndChangeCheck())
      {
        RegenerateTerrain();
      }
    }

    public static void DrawSceneGUI()
    {
      FindTerrain();

      if (renderer == null)
        return;

      Vector3 center =
        new Vector3(
          terrainData.ChunksX *
          terrainData.ChunkSize * 0.5f,
          terrainData.HeightMultiplier * 0.5f,
          terrainData.ChunksZ *
          terrainData.ChunkSize * 0.5f
        );

      EditorGUI.BeginChangeCheck();

      Vector3 scale =
        Handles.ScaleHandle(
          new Vector3(
            terrainData.ChunksX,
            terrainData.HeightMultiplier,
            terrainData.ChunksZ
          ),
          center,
          Quaternion.identity,
          HandleUtility.GetHandleSize(center)
        );

      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(
          terrainData,
          "Resize Terrain");

        terrainData.ChunksX =
          Mathf.Max(1, Mathf.RoundToInt(scale.x));

        terrainData.ChunksZ =
          Mathf.Max(1, Mathf.RoundToInt(scale.z));

        terrainData.HeightMultiplier =
          Mathf.Max(1, scale.y);

        RegenerateTerrain();
      }
    }

    private static void FindTerrain()
    {
      if (renderer != null)
        return;

      renderer =
        Object.FindObjectOfType<TerrainWorldRenderer>();

      if (renderer == null)
        return;

      SerializedObject so =
        new SerializedObject(renderer);

      SerializedProperty property =
        so.FindProperty("terrainData");

      terrainData =
        property.objectReferenceValue
          as TerrainDataAsset;
    }

    private static void RegenerateTerrain()
    {
      if (renderer == null)
        return;

      renderer.Generate();

      EditorUtility.SetDirty(terrainData);
    }
  }
}