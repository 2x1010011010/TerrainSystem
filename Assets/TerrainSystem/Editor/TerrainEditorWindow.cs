#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerrainSystem.Editor
{
    /// <summary>
    /// Custom Inspector for TerrainGenerator — renders inside Unity's Inspector tab.
    /// Shows three focused panels: Chunks (X/Z), Height (Y), and Noise.
    /// </summary>
    [CustomEditor(typeof(TerrainGenerator))]
    public class TerrainInspector : UnityEditor.Editor
    {
        SerializedObject _settingsSO;

        // ------------------------------------------------------------------ //
        //  Colours
        // ------------------------------------------------------------------ //

        static readonly Color s_Bg       = new Color(0.13f, 0.13f, 0.15f, 1f);
        static readonly Color s_PanelBg  = new Color(0.17f, 0.17f, 0.20f, 1f);
        static readonly Color s_HeaderBg = new Color(0.20f, 0.20f, 0.23f, 1f);
        static readonly Color s_Border   = new Color(0.27f, 0.27f, 0.31f, 1f);
        static readonly Color s_Accent   = new Color(0.28f, 0.72f, 0.44f, 1f);
        static readonly Color s_TextMain = new Color(0.88f, 0.88f, 0.92f, 1f);
        static readonly Color s_TextDim  = new Color(0.52f, 0.52f, 0.57f, 1f);
        static readonly Color s_Warning  = new Color(1.00f, 0.74f, 0.32f, 1f);

        // ------------------------------------------------------------------ //
        //  Lifecycle
        // ------------------------------------------------------------------ //

        void OnEnable() => RefreshSettingsSO();

        void RefreshSettingsSO()
        {
            var gen = (TerrainGenerator)target;
            _settingsSO = (gen != null && gen.settings != null)
                ? new SerializedObject(gen.settings)
                : null;
        }

        // ------------------------------------------------------------------ //
        //  CreateInspectorGUI
        // ------------------------------------------------------------------ //

        public override VisualElement CreateInspectorGUI()
        {
            var gen = (TerrainGenerator)target;

            var root = new VisualElement();
            root.style.backgroundColor         = s_Bg;
            root.style.borderTopLeftRadius     = 6;
            root.style.borderTopRightRadius    = 6;
            root.style.borderBottomLeftRadius  = 6;
            root.style.borderBottomRightRadius = 6;
            root.style.overflow    = Overflow.Hidden;
            root.style.marginTop   = 2;
            root.style.marginBottom = 6;

            root.Add(BuildHeader(gen));

            var body = new VisualElement();
            body.style.paddingLeft   = 6;
            body.style.paddingRight  = 6;
            body.style.paddingBottom = 8;
            root.Add(body);

            body.Add(BuildSettingsField(gen));

            if (_settingsSO != null)
            {
                body.Add(BuildChunksPanel());
                body.Add(BuildHeightPanel());
                body.Add(BuildNoisePanel());
                body.Add(BuildFooter(gen));
                root.Bind(_settingsSO);
            }
            else
            {
                body.Add(BuildWarningBanner("Assign a TerrainSettings asset above to edit parameters."));
                body.Add(BuildFooter(gen));
            }

            return root;
        }

        // ------------------------------------------------------------------ //
        //  Header
        // ------------------------------------------------------------------ //

        VisualElement BuildHeader(TerrainGenerator gen)
        {
            var header = new VisualElement();
            header.style.backgroundColor   = new Color(0.10f, 0.10f, 0.12f, 1f);
            header.style.flexDirection     = FlexDirection.Row;
            header.style.alignItems        = Align.Center;
            header.style.paddingTop        = 11;
            header.style.paddingBottom     = 11;
            header.style.paddingLeft       = 14;
            header.style.paddingRight      = 14;
            header.style.borderBottomWidth = 1;
            header.style.borderBottomColor = s_Border;

            var icon = new Label("⛰");
            icon.style.fontSize    = 18;
            icon.style.marginRight = 10;

            var col = new VisualElement { style = { flexGrow = 1 } };

            var title = new Label("TERRAIN GENERATOR");
            title.style.color       = s_TextMain;
            title.style.fontSize    = 11;
            title.style.letterSpacing = 1.1f;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;

            var sub = new Label(gen.name);
            sub.style.color     = s_Accent;
            sub.style.fontSize  = 10;
            sub.style.marginTop = 2;

            col.Add(title);
            col.Add(sub);

            header.Add(icon);
            header.Add(col);
            return header;
        }

        // ------------------------------------------------------------------ //
        //  Settings asset reference
        // ------------------------------------------------------------------ //

        VisualElement BuildSettingsField(TerrainGenerator gen)
        {
            var row = MakeRow("Settings Asset", "", 0);

            var field = new ObjectField { objectType = typeof(TerrainSettings), label = "" };
            field.style.flexGrow = 1;
            field.value = gen.settings;
            field.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(gen, "Change Terrain Settings");
                gen.settings = (TerrainSettings)evt.newValue;
                EditorUtility.SetDirty(gen);
                RefreshSettingsSO();
                ActiveEditorTracker.sharedTracker.ForceRebuild();
            });
            row.Add(field);
            return row;
        }

        // ------------------------------------------------------------------ //
        //  Panel: CHUNKS  (X and Z chunk count)
        // ------------------------------------------------------------------ //

        VisualElement BuildChunksPanel() =>
            BuildPanel("CHUNKS", "▦", "Number of chunks that tile the terrain footprint", content =>
            {
                if (_settingsSO == null) return;

                // We expose chunkCount.x and chunkCount.y as individual int fields
                // by binding to the Vector2Int property and letting PropertyField handle it.
                // For a cleaner look, show them as two labelled rows.
                var prop = _settingsSO.FindProperty("chunkCount");

                content.Add(BuildIntRow(
                    "Chunks X", "chunkCount",
                    "Number of chunks along the X axis",
                    v => {
                        var p = _settingsSO.FindProperty("chunkCount");
                        p.vector2IntValue = new Vector2Int(Mathf.Max(1, v), p.vector2IntValue.y);
                        _settingsSO.ApplyModifiedProperties();
                    },
                    () => _settingsSO.FindProperty("chunkCount").vector2IntValue.x
                ));

                content.Add(BuildIntRow(
                    "Chunks Z", "chunkCount",
                    "Number of chunks along the Z axis",
                    v => {
                        var p = _settingsSO.FindProperty("chunkCount");
                        p.vector2IntValue = new Vector2Int(p.vector2IntValue.x, Mathf.Max(1, v));
                        _settingsSO.ApplyModifiedProperties();
                    },
                    () => _settingsSO.FindProperty("chunkCount").vector2IntValue.y
                ));

                // Live total size readout
                var info = MakeInfoLabel("");
                info.schedule.Execute(() =>
                {
                    _settingsSO?.Update();
                    var gen = target as TerrainGenerator;
                    if (gen?.settings == null) return;
                    var s = gen.settings;
                    info.text = $"Footprint  {s.TotalSize.x:F1} × {s.TotalSize.y:F1} m";
                }).Every(200);
                content.Add(info);
            });

        // Custom int row that reads/writes a single axis of a Vector2Int property
        VisualElement BuildIntRow(string label, string bindPath, string tooltip,
                                   System.Action<int> setter, System.Func<int> getter)
        {
            var row = MakeRow(label, tooltip, 4);

            var field = new IntegerField { label = "" };
            field.style.flexGrow = 1;
            field.value = getter();

            // Keep display fresh when settings change externally
            field.schedule.Execute(() => field.SetValueWithoutNotify(getter())).Every(300);

            field.RegisterValueChangedCallback(evt =>
            {
                if (_settingsSO == null) return;
                Undo.RecordObject(_settingsSO.targetObject, $"Edit {label}");
                setter(evt.newValue);
                EditorUtility.SetDirty(_settingsSO.targetObject);
            });

            row.Add(field);
            return row;
        }

        // ------------------------------------------------------------------ //
        //  Panel: HEIGHT  (Y axis — height scale + multiplier)
        // ------------------------------------------------------------------ //

        VisualElement BuildHeightPanel() =>
            BuildPanel("HEIGHT", "↑", "Controls how tall the terrain mountains are", content =>
            {
                if (_settingsSO == null) return;

                content.Add(MakePropertyRow("Height Scale",      "heightScale",
                    "Base height in world units. Scaled by Height Multiplier."));
                content.Add(MakePropertyRow("Height Multiplier", "heightMultiplier",
                    "Multiplier applied on top of Height Scale. Also driven by the Y gizmo handle."));

                // Preview: effective peak height
                var info = MakeInfoLabel("");
                info.schedule.Execute(() =>
                {
                    _settingsSO?.Update();
                    var gen = target as TerrainGenerator;
                    if (gen?.settings == null) return;
                    var s = gen.settings;
                    info.text = $"Peak height  ≈ {s.heightScale * s.heightMultiplier:F2} m";
                }).Every(200);
                content.Add(info);
            });

        // ------------------------------------------------------------------ //
        //  Panel: NOISE  (single Noise Scale slider + seed)
        // ------------------------------------------------------------------ //

        VisualElement BuildNoisePanel() =>
            BuildPanel("NOISE", "〰", "Perlin fBm noise that shapes the terrain surface", content =>
            {
                if (_settingsSO == null) return;

                // Primary control: noise scale
                content.Add(MakePropertyRow("Noise Scale",  "noiseScale",
                    "Zoom of the noise pattern — lower = broader hills, higher = jagged detail."));
                content.Add(MakePropertyRow("Seed",         "noiseSeed",    "Random seed for noise offsets."));
                content.Add(MakePropertyRow("Offset",       "noiseOffset",  "Pan the noise map in world space."));
                content.Add(MakePropertyRow("Octaves",      "octaves",      "Number of stacked noise layers."));
                content.Add(MakePropertyRow("Persistence",  "persistence",  "How fast amplitude drops per octave."));
                content.Add(MakePropertyRow("Lacunarity",   "lacunarity",   "How fast frequency grows per octave."));
            });

        // ------------------------------------------------------------------ //
        //  Footer
        // ------------------------------------------------------------------ //

        VisualElement BuildFooter(TerrainGenerator gen)
        {
            var footer = new VisualElement();
            footer.style.flexDirection  = FlexDirection.Row;
            footer.style.alignItems     = Align.Center;
            footer.style.justifyContent = Justify.SpaceBetween;
            footer.style.marginTop      = 8;
            footer.style.paddingLeft    = 4;
            footer.style.paddingRight   = 4;

            var toggle = new Toggle("Auto Regenerate");
            StyleToggle(toggle);
            toggle.value = gen.autoRegenerate;
            toggle.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(gen, "Toggle Auto Regenerate");
                gen.autoRegenerate = evt.newValue;
                EditorUtility.SetDirty(gen);
            });

            var btn = MakePrimaryButton("⟳  GENERATE", () =>
            {
                if (gen == null || gen.settings == null) return;
                Undo.RecordObject(gen.settings, "Generate Terrain");
                gen.Generate();
                EditorUtility.SetDirty(gen.settings);
            });

            footer.Add(toggle);
            footer.Add(btn);
            return footer;
        }

        // ------------------------------------------------------------------ //
        //  Panel builder
        // ------------------------------------------------------------------ //

        VisualElement BuildPanel(string title, string icon, string tooltip,
                                  System.Action<VisualElement> populate,
                                  bool startExpanded = true)
        {
            bool expanded = startExpanded;

            var panel = new VisualElement();
            panel.tooltip = tooltip;
            panel.style.marginTop   = 6;
            panel.style.overflow    = Overflow.Hidden;
            panel.style.backgroundColor          = s_PanelBg;
            panel.style.borderTopLeftRadius      = 5;
            panel.style.borderTopRightRadius     = 5;
            panel.style.borderBottomLeftRadius   = 5;
            panel.style.borderBottomRightRadius  = 5;
            panel.style.borderTopWidth    = 1; panel.style.borderTopColor    = s_Border;
            panel.style.borderLeftWidth   = 1; panel.style.borderLeftColor   = s_Border;
            panel.style.borderRightWidth  = 1; panel.style.borderRightColor  = s_Border;
            panel.style.borderBottomWidth = 1; panel.style.borderBottomColor = s_Border;

            // Header row
            var hdr = new VisualElement();
            hdr.style.flexDirection  = FlexDirection.Row;
            hdr.style.alignItems     = Align.Center;
            hdr.style.backgroundColor = s_HeaderBg;
            hdr.style.paddingTop     = 7;
            hdr.style.paddingBottom  = 7;
            hdr.style.paddingLeft    = 10;
            hdr.style.paddingRight   = 10;

            var arrow = new Label(expanded ? "▾" : "▸");
            arrow.style.color       = s_Accent;
            arrow.style.fontSize    = 11;
            arrow.style.marginRight = 5;
            arrow.style.unityFontStyleAndWeight = FontStyle.Bold;

            var ico = new Label(icon);
            ico.style.fontSize    = 13;
            ico.style.marginRight = 5;

            var ttl = new Label(title);
            ttl.style.color       = s_TextMain;
            ttl.style.fontSize    = 10;
            ttl.style.letterSpacing = 0.7f;
            ttl.style.unityFontStyleAndWeight = FontStyle.Bold;

            hdr.Add(arrow); hdr.Add(ico); hdr.Add(ttl);
            panel.Add(hdr);

            // Content
            var content = new VisualElement();
            content.style.paddingTop    = 9;
            content.style.paddingBottom = 11;
            content.style.paddingLeft   = 10;
            content.style.paddingRight  = 10;
            content.style.display = expanded ? DisplayStyle.Flex : DisplayStyle.None;
            populate(content);
            panel.Add(content);

            hdr.RegisterCallback<ClickEvent>(_ =>
            {
                expanded = !expanded;
                content.style.display = expanded ? DisplayStyle.Flex : DisplayStyle.None;
                arrow.text = expanded ? "▾" : "▸";
            });

            return panel;
        }

        // ------------------------------------------------------------------ //
        //  Small UI helpers
        // ------------------------------------------------------------------ //

        // Two-column row: fixed label left, flex field right
        VisualElement MakeRow(string labelText, string tooltip, float marginBottom)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems    = Align.Center;
            row.style.marginBottom  = marginBottom;
            row.style.marginTop     = 6;
            row.tooltip             = tooltip;

            if (!string.IsNullOrEmpty(labelText))
            {
                var lbl = new Label(labelText);
                lbl.style.color      = s_TextDim;
                lbl.style.fontSize   = 10;
                lbl.style.minWidth   = 110;
                lbl.style.flexShrink = 0;
                row.Add(lbl);
            }

            return row;
        }

        VisualElement MakePropertyRow(string labelText, string bindPath, string tooltip)
        {
            var row = MakeRow(labelText, tooltip, 4);
            var f   = new PropertyField { bindingPath = bindPath, label = "" };
            f.style.flexGrow = 1;
            row.Add(f);
            return row;
        }

        Label MakeInfoLabel(string text)
        {
            var lbl = new Label(text);
            lbl.style.color     = s_TextDim;
            lbl.style.fontSize  = 9;
            lbl.style.marginTop = 5;
            return lbl;
        }

        VisualElement BuildWarningBanner(string msg)
        {
            var banner = new VisualElement();
            banner.style.marginTop    = 8;
            banner.style.paddingTop   = 9; banner.style.paddingBottom = 9;
            banner.style.paddingLeft  = 12; banner.style.paddingRight = 12;
            banner.style.backgroundColor         = new Color(0.22f, 0.15f, 0.08f, 1f);
            banner.style.borderTopLeftRadius     = 5;
            banner.style.borderTopRightRadius    = 5;
            banner.style.borderBottomLeftRadius  = 5;
            banner.style.borderBottomRightRadius = 5;
            var lbl = new Label($"⚠  {msg}");
            lbl.style.color      = s_Warning;
            lbl.style.fontSize   = 10;
            lbl.style.whiteSpace = WhiteSpace.Normal;
            banner.Add(lbl);
            return banner;
        }

        static Button MakePrimaryButton(string text, System.Action onClick)
        {
            var btn = new Button(onClick) { text = text };
            btn.style.backgroundColor = new Color(0.28f, 0.72f, 0.44f, 1f);
            btn.style.color           = new Color(0.06f, 0.06f, 0.06f, 1f);
            btn.style.unityFontStyleAndWeight = FontStyle.Bold;
            btn.style.fontSize        = 10;
            btn.style.paddingTop      = 6; btn.style.paddingBottom = 6;
            btn.style.paddingLeft     = 14; btn.style.paddingRight = 14;
            btn.style.borderTopLeftRadius     = 4;
            btn.style.borderTopRightRadius    = 4;
            btn.style.borderBottomLeftRadius  = 4;
            btn.style.borderBottomRightRadius = 4;
            btn.style.borderTopWidth    = 0; btn.style.borderLeftWidth   = 0;
            btn.style.borderRightWidth  = 0; btn.style.borderBottomWidth = 0;
            return btn;
        }

        static void StyleToggle(Toggle t)
        {
            t.style.color   = new Color(0.52f, 0.52f, 0.57f, 1f);
            t.style.fontSize = 10;
        }
    }
}
#endif
