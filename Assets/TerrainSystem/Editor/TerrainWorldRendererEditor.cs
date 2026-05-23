using TerrainSystem.Runtime.Data;
using TerrainSystem.Runtime.Rendering;
using UnityEditor;
using UnityEngine;

namespace TerrainSystem.Editor
{
  [CustomEditor(typeof(TerrainWorldRenderer))]
  public class TerrainWorldRendererEditor : UnityEditor.Editor
  {
    private TerrainWorldRenderer renderer;
    private TerrainDataAsset terrainData;

    private int selectedTab;

    private readonly string[] tabs =
    {
      "Size",
      "Paint",
      "Biome",
      "Water"
    };

    private void OnEnable()
    {
      renderer =
        (TerrainWorldRenderer)target;

      terrainData =
        GetTerrainData();

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
      if (terrainData == null)
      {
        EditorGUILayout.HelpBox(
          "TerrainData missing.",
          MessageType.Error);

        return;
      }

      GUILayout.Space(10);

      selectedTab =
        GUILayout.Toolbar(selectedTab, tabs);

      GUILayout.Space(10);

      switch (selectedTab)
      {
        case 0:
          DrawSizeTab();
          break;

        case 1:
          DrawPaintTab();
          break;

        case 2:
          DrawBiomeTab();
          break;

        case 3:
          DrawWaterTab();
          break;
      }
    }

    #region SIZE TAB

    private void DrawSizeTab()
    {
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
        "Height",
        EditorStyles.boldLabel);

      terrainData.HeightMultiplier =
        EditorGUILayout.FloatField(
          "Height",
          terrainData.HeightMultiplier);

      GUILayout.Space(10);

      GUILayout.Label(
        "Surface",
        EditorStyles.boldLabel);

      terrainData.NoiseScale =
        EditorGUILayout.Slider(
          "Noise Scale",
          terrainData.NoiseScale,
          0.001f,
          1f);

      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(
          terrainData,
          "Resize Terrain");

        RegenerateTerrain();
      }
    }

    #endregion

    #region PAINT TAB

    private void DrawPaintTab()
    {
      GUILayout.Label(
        "Paint Tool",
        EditorStyles.boldLabel);
    }

    #endregion

    #region BIOME TAB

    private void DrawBiomeTab()
    {
      GUILayout.Label(
        "Biome Tool",
        EditorStyles.boldLabel);
    }

    #endregion

    #region WATER TAB

    private void DrawWaterTab()
    {
      GUILayout.Label(
        "Water Tool",
        EditorStyles.boldLabel);
    }

    #endregion

    #region SCENE GUI

    private void OnSceneGUI(SceneView sceneView)
    {
      if (renderer == null ||
          terrainData == null)
      {
        return;
      }

      switch (selectedTab)
      {
        case 0:
          DrawSizeHandles();
          break;
      }
    }

    private void DrawSizeHandles()
    {
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
          Mathf.Max(1,
            Mathf.RoundToInt(scale.x));

        terrainData.ChunksZ =
          Mathf.Max(1,
            Mathf.RoundToInt(scale.z));

        terrainData.HeightMultiplier =
          Mathf.Max(1f,
            scale.y);

        RegenerateTerrain();
      }
    }

    #endregion

    #region HELPERS

    private TerrainDataAsset GetTerrainData()
    {
      return renderer.TerrainData;
    }

    private void RegenerateTerrain()
    {
      renderer.Generate();

      EditorUtility.SetDirty(
        terrainData);
    }

    #endregion
  }
}