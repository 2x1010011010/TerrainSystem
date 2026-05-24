#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace TerrainSystem.Editor
{
    /// <summary>
    /// Draws a custom Scale-style gizmo in the Scene view for TerrainGenerator.
    /// • Dragging the X / Z handles resizes the terrain footprint.
    /// • Dragging the Y handle changes the height multiplier only (mesh stays same size).
    /// • Unity's built-in Move / Rotate / Scale tools are suppressed while this object is selected.
    /// </summary>
    [CustomEditor(typeof(TerrainGenerator))]
    public class TerrainGizmoHandle : UnityEditor.Editor
    {
        // ---- visual constants ---------------------------------------------
        const float HANDLE_SIZE      = 0.12f;  // fraction of handle distance
        const float CENTRE_SIZE      = 0.10f;
        const float LINE_THICKNESS   = 2.5f;
        const float MIN_DIMENSION    = 0.5f;

        static readonly Color COLOR_X      = new Color(0.95f, 0.25f, 0.25f, 1f);
        static readonly Color COLOR_Y      = new Color(0.35f, 0.90f, 0.25f, 1f);
        static readonly Color COLOR_Z      = new Color(0.25f, 0.50f, 0.95f, 1f);
        static readonly Color COLOR_CENTRE = new Color(0.90f, 0.90f, 0.90f, 1f);
        static readonly Color COLOR_FILL   = new Color(1f, 1f, 1f, 0.04f);

        // ---- state --------------------------------------------------------
        Vector3 _sizeAtDragStart;
        bool    _dragging;

        // ------------------------------------------------------------------ //
        //  Suppress Unity's default tools
        // ------------------------------------------------------------------ //

        public override void OnInspectorGUI() { /* handled by TerrainEditorWindow */ }

        // Hide the default tools while our generator is selected
        Tool _previousTool = Tool.None;

        void OnEnable()
        {
            _previousTool       = Tools.current;
            Tools.hidden        = true;
        }

        void OnDisable()
        {
            Tools.hidden = false;
        }

        // ------------------------------------------------------------------ //
        //  Scene GUI
        // ------------------------------------------------------------------ //

        void OnSceneGUI()
        {
            TerrainGenerator gen = (TerrainGenerator)target;
            if (gen == null || gen.settings == null) return;

            Vector3 worldPos = gen.transform.position;
            Vector3 size     = gen.GetSize(); // (totalX, heightMul, totalZ)

            // Half-extents for handle placement
            float hx = size.x * 0.5f;
            float hy = size.y * 0.5f;   // used only for the Y handle arm length
            float hz = size.z * 0.5f;

            // Handle positions in world space (Y handle sits at terrain "top")
            Vector3 posX = worldPos + new Vector3( hx,   0f,   0f);
            Vector3 negX = worldPos + new Vector3(-hx,   0f,   0f);
            Vector3 posZ = worldPos + new Vector3(  0f,  0f,  hz);
            Vector3 negZ = worldPos + new Vector3(  0f,  0f, -hz);
            Vector3 posY = worldPos + new Vector3(  0f,  hy * gen.settings.heightScale,  0f);

            DrawFootprintRect(worldPos, hx, hz);

            EditorGUI.BeginChangeCheck();

            // ---- X handles ------------------------------------------------
            float newHX = DrawScaleHandle(posX, hx, Vector3.right,   COLOR_X, size, "X+");
            float newHX2 = DrawScaleHandle(negX, hx, Vector3.left,   COLOR_X, size, "X-");
            float newHZ = DrawScaleHandle(posZ, hz, Vector3.forward,  COLOR_Z, size, "Z+");
            float newHZ2 = DrawScaleHandle(negZ, hz, Vector3.back,    COLOR_Z, size, "Z-");
            float newHY = DrawYHandle(posY, hy * gen.settings.heightScale, COLOR_Y, size);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(gen.settings, "Resize Terrain");

                // Average symmetric pairs so both sides move together
                float finalX  = Mathf.Max(Mathf.Max(newHX, newHX2) * 2f, MIN_DIMENSION);
                float finalZ  = Mathf.Max(Mathf.Max(newHZ, newHZ2) * 2f, MIN_DIMENSION);
                float finalHY = Mathf.Max(newHY, 0.01f);
                float finalY  = finalHY / gen.settings.heightScale;

                gen.ApplySize(new Vector3(finalX, finalY, finalZ));
                EditorUtility.SetDirty(gen.settings);
            }

            // Centre dot
            Handles.color = COLOR_CENTRE;
            Handles.SphereHandleCap(0, worldPos, Quaternion.identity,
                HandleUtility.GetHandleSize(worldPos) * CENTRE_SIZE, EventType.Repaint);

            DrawLabels(worldPos, size, posX, posZ, posY);
        }

        // ------------------------------------------------------------------ //
        //  Sub-drawing helpers
        // ------------------------------------------------------------------ //

        /// Returns the NEW half-extent (same axis) after the user drags.
        float DrawScaleHandle(Vector3 handlePos, float currentHalf,
                              Vector3 axis, Color col, Vector3 size, string id)
        {
            float handleSize = HandleUtility.GetHandleSize(handlePos) * HANDLE_SIZE;

            Handles.color = col;
            // Axis line from centre
            Handles.DrawAAPolyLine(LINE_THICKNESS,
                handlePos - axis * currentHalf, handlePos);

            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.Slider(handlePos, axis, handleSize,
                                            Handles.CubeHandleCap, 0f);

            if (EditorGUI.EndChangeCheck())
            {
                // Distance from centre → new half-extent
                Vector3 origin = handlePos - axis * currentHalf;
                float newHalf = Vector3.Dot(newPos - origin, axis);
                return Mathf.Max(newHalf, MIN_DIMENSION * 0.5f);
            }

            return currentHalf;
        }

        /// Separate handler for Y so we can scale by heightScale
        float DrawYHandle(Vector3 handlePos, float armLength, Color col, Vector3 size)
        {
            Vector3 origin     = handlePos - Vector3.up * armLength;
            float   handleSize = HandleUtility.GetHandleSize(handlePos) * HANDLE_SIZE;

            Handles.color = col;
            Handles.DrawAAPolyLine(LINE_THICKNESS, origin, handlePos);

            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.Slider(handlePos, Vector3.up, handleSize,
                                            Handles.CubeHandleCap, 0f);

            if (EditorGUI.EndChangeCheck())
            {
                float newArm = Mathf.Max(Vector3.Dot(newPos - origin, Vector3.up), 0.05f);
                return newArm;
            }

            return armLength;
        }

        void DrawFootprintRect(Vector3 centre, float hx, float hz)
        {
            Vector3 p0 = centre + new Vector3(-hx, 0f, -hz);
            Vector3 p1 = centre + new Vector3( hx, 0f, -hz);
            Vector3 p2 = centre + new Vector3( hx, 0f,  hz);
            Vector3 p3 = centre + new Vector3(-hx, 0f,  hz);

            // Filled quad
            Handles.color = COLOR_FILL;
            Handles.DrawSolidRectangleWithOutline(
                new Vector3[]{ p0, p1, p2, p3 }, COLOR_FILL, Color.clear);

            // Outline
            Handles.color = new Color(1f, 1f, 1f, 0.25f);
            Handles.DrawAAPolyLine(LINE_THICKNESS, p0, p1, p2, p3, p0);
        }

        void DrawLabels(Vector3 origin, Vector3 size,
                        Vector3 posX, Vector3 posZ, Vector3 posY)
        {
            GUIStyle style = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

            Handles.Label(posX + Vector3.right   * 0.3f, $"W: {size.x:F1}", style);
            Handles.Label(posZ + Vector3.forward * 0.3f, $"D: {size.z:F1}", style);
            Handles.Label(posY + Vector3.up      * 0.3f, $"H: {size.y:F2}x", style);
        }
    }
}
#endif
