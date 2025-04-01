using System;
using System.IO;
using System.Text;
using AudioTextSynchronizer.Core;
using AudioTextSynchronizer.Editor.Whisper;
using AudioTextSynchronizer.Tools;
using AudioTextSynchronizer.Tools.Subtitles;
using UnityEditor;
using UnityEngine;

namespace AudioTextSynchronizer.Editor.Timings
{
    public class TimingsWindow : EditorWindow
    {
        public static TimingsWindow instance;
        private static WhisperEditorHelper whisperEditorHelper;
        public PhraseAsset Data;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private PhraseAsset loadedAsset;
        [SerializeField] private string timingsAssetPath = string.Empty;
        private Input input;
        private Curve curve;
        private Timeline timeline;
        private Timings timings;
        private TimingsValidator timingsValidator;
        private Timing audioPlayingTiming;
        private bool isPlayingTiming;
        private bool isPlaying;
        private bool isDataLoaded;
        private GenericMenu menu;
        private Rect curvePanel;
        private Rect timingsRect;
        private Rect curveRect;
        private Rect clickRect;
        private Rect curveRectFull;
        private Rect doubleClickRect;
        private Rect resizeRect;
        private Vector2 curvePadding = new Vector2(30f, 30);
        private Vector2 scrollTextAreaPosition;
        private Vector2 scrollPosition;
        private bool isResize;
        private bool autoSave;
        private bool isTimelineDragStarted;
        private double clickTime;
        private float curveSize;
        private float curveHeight;

        private bool AutoSave
        {
            get => autoSave && timingsAssetPath != string.Empty;
            set
            {
                autoSave = value;
                EditorPrefs.SetBool("ATS_AutoSave", autoSave);
            }
        }

        private readonly Color audioCursorColor = new Color(203 / 255f, 88 / 255f, 88 / 255f, 255 / 255f);
        private const double DoubleClickTime = 0.3;
        private const float SeparatorHeight = 6f;
        private const float MinCurveHeight = 100f;
        public const float LabelWidth = 75f;
        public const float ScrollHeight = 32f;
        public const string Title = "Timings Editor";
        
        [MenuItem("Window/Audio Text Synchronizer/Timings Editor", false, 111)]
        private static void OpenWindow()
        {
            instance = GetWindow<TimingsWindow>(Title, true);
            if (whisperEditorHelper == null)
            {
                whisperEditorHelper = new WhisperEditorHelper();
            }
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnMouseDrag(int controlId)
        {
            if (GUIUtility.hotControl == controlId)
            {
                if (Data != null)
                {
                    Undo.RecordObject(Data, "Moved Timings");
                }
                timings.OnDragTiming(curveRect.width, curvePadding.x);
                Event.current.Use();
            }
        }

        private void OnMouseWheel()
        {
            if (Mathf.Abs(Event.current.delta.y) > 2f && doubleClickRect.Contains(Event.current.mousePosition))
            {
                GUIUtility.hotControl = 0;
                Event.current.Use();
                curveSize -= Event.current.delta.y / 100f;
                if (curveSize < 1f)
                {
                    curveSize = 1f;
                }
            }
        }

        private void OnMouseDown(int controlId)
        {
            if (curvePanel.Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 0)
                {
                    timings.OnMouseDown(curvePanel.height);
                    GUIUtility.hotControl = controlId;
                    Event.current.Use();
                }
                
                if (Event.current.button == 1)
                {
                    var xPosition = Event.current.mousePosition.x;
                    menu = new GenericMenu();
                    var selectedTimingsCount = timings.GetSelectedTimingsCount();
                    var onTimingHover = selectedTimingsCount > 0;
                    if (selectedTimingsCount < 2)
                    {
                        AddMenuItem("Play", onTimingHover, PlayTiming);
                        AddMenuItem("Insert new timing", true, () =>
                        {
                            SetCursorPosition(xPosition);
                            AddTiming(xPosition);
                        });
                        AddMenuItem("Remove timing", onTimingHover, RemoveTiming);
                        menu.ShowAsContext();
                    }
                    else
                    {
                        AddMenuItem("Play", false, PlayTiming);
                        AddMenuItem("Insert new timing", false, () =>
                        {
                            SetCursorPosition(xPosition);
                            AddTiming(xPosition);
                        });
                        AddMenuItem("Remove timings", onTimingHover, RemoveTiming);
                        menu.ShowAsContext();
                    }
                }
            }
        }

        private void AddMenuItem(string text, bool enabled, GenericMenu.MenuFunction onAction = null)
        {
            if (enabled)
            {
                menu.AddItem(new GUIContent(text), false, onAction);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(text), false);
            }
        }

        private void OnDestroy()
        {
            if (input != null)
            {
                input.OnKeyDown -= timings.OnKeyDown;
                input.OnKeyDown -= OnKeyDown;
            }
            audioManager.Destroy();
        }

        private void OnGUI()
        {
            DrawTopPanel();
            if (Data.Clip != null)
            {
                CalculateRects();
                EditorGUILayout.BeginHorizontal();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(curveHeight));
                input.Update();
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
                timeline.Draw(Data.Clip.length);
                EditorGUILayout.EndHorizontal();
                GUILayoutUtility.GetRect(curveRect.width + curvePadding.x * 2f, curveHeight - curvePadding.y);
                curve.Render();
                timings.Draw();
                OnTimelineDrag();
                OnDoubleClick();
                DrawAudioCursor();
#if ATS_DEBUG
                // EditorGUI.DrawRect(curvePanel, new Color(0, 0, 1, 0.2f));
                // EditorGUI.DrawRect(timingsRect, new Color(1, 0, 0, 0.2f));
                // EditorGUI.DrawRect(clickRect, new Color(1, 0, 1, 0.3f));
                // EditorGUI.DrawRect(doubleClickRect, new Color(0, 1, 1, 0.5f));
                // EditorGUI.DrawRect(curveRectFull, new Color(1, 0.2f, 0.4f, 0.5f));
#endif
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndHorizontal();
                DrawResizeRect();
            }
            DrawBottomPanel();
            DrawSeparator();
            DrawAudioClipField();
            timings.DrawInfo();
            DrawTextField();
        }

        private void Update()
        {
            Repaint();
            
            if (audioManager.Source != null && audioManager.Source.isPlaying)
            {
                if (isPlayingTiming && audioPlayingTiming != null && audioManager.Source.time >= audioPlayingTiming.EndPosition)
                {
                    audioManager.Source.Pause();
                    isPlayingTiming = false;
                }
            }
        }

        private void CalculateRects()
        {
            if (Mathf.Abs(curveHeight) < 1f)
            {
                curveHeight = position.height * 0.4f;
            }

            curveHeight = Mathf.Clamp(curveHeight, MinCurveHeight, position.height * 0.5f);
            curvePadding = new Vector2(30f, EditorStyles.toolbar.fixedHeight);
            curvePanel = new Rect(curvePadding.x, curvePadding.y, position.width * curveSize, curveHeight);
            timingsRect = new Rect(curvePadding.x, curvePadding.y, position.width * curveSize, curveHeight - curvePadding.y);
            curveRect = new Rect(curvePadding.x, curvePadding.y, position.width * curveSize - curvePadding.x * 2f, curveHeight - curvePadding.y * 2f);
            curveRectFull = new Rect(curvePadding.x, 0, position.width * curveSize - curvePadding.x * 2f, curveHeight);
            clickRect = new Rect(0, 0, position.width * curveSize, curvePadding.y);
            doubleClickRect = new Rect(0, curvePadding.y, position.width * curveSize, curveHeight - curvePadding.y);
            resizeRect = new Rect(0, curvePanel.height + curvePadding.y + EditorStyles.toolbarButton.fixedHeight + 3f, position.width, SeparatorHeight);
        }

        private void DrawResizeRect()
        {
            if (Event.current.type == EventType.MouseDown && resizeRect.Contains(Event.current.mousePosition))
            {
                isResize = true;
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                isResize = false;
            }
            if (isResize)
            {
                curveHeight = Event.current.mousePosition.y - curvePadding.y * 1.5f + SeparatorHeight / 2f;
                curveHeight = Mathf.Clamp(curveHeight, MinCurveHeight, position.height);
                resizeRect = new Rect(0, curveRect.height + curvePadding.y * 3f + 9f, position.width, SeparatorHeight);
                Repaint();
            }
            EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeVertical);
        }

        private void OnTimelineDrag()
        {
            if (Event.current.button == 0)
            {
                if (Event.current.type == EventType.MouseDrag && clickRect.Contains(Event.current.mousePosition))
                {
                    isTimelineDragStarted = true;
                }
                
                if (curveRectFull.Contains(Event.current.mousePosition) && isTimelineDragStarted && Event.current.type != EventType.MouseUp)
                {
                    SetCursorPosition(Event.current.mousePosition.x);
                }
                else
                {
                    isTimelineDragStarted = false;
                }
            }
        }
        
        private void OnDoubleClick()
        {
            if (Event.current.button == 0 && Event.current.clickCount > 0 && doubleClickRect.Contains(Event.current.mousePosition))
            {
                if (EditorApplication.timeSinceStartup - clickTime < DoubleClickTime && Event.current.clickCount % 2 == 0)
                {
                    SetCursorPosition(Event.current.mousePosition.x);
                    Event.current.clickCount = 0;
                }
                clickTime = EditorApplication.timeSinceStartup;
            }
        }

        private void SetCursorPosition(float xPosition)
        {
            var time = (xPosition - curvePadding.x) / curveRect.width * Data.Clip.length;
            time = Mathf.Clamp(time, 0f, Data.Clip.length - 0.0001f);
            if (!audioManager.IsStartPlaying())
            {
                audioManager.Source.Play();
            }
            audioManager.Source.Pause();
            audioManager.SetPosition(time);
        }

        private async void DrawTopPanel()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            if (GUILayout.Button(new GUIContent("New", "Create a new timings"), EditorStyles.toolbarButton) && Data != null && Data.Timings.Count > 0 &&
                EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to create new timings? All your current changes will be lost.", "Yes", "Cancel"))
            {
                CreateAsset();
                timings.Deselect();
            }

            if (GUILayout.Button(new GUIContent("Load", "Load existing asset with timings"), EditorStyles.toolbarButton))
            {
                var path = EditorUtility.OpenFilePanelWithFilters("Title", "Directory", new []{"Asset files", "asset", "Subtitle files", "srt"});
                LoadAsset(path);
            }

            if ((!string.IsNullOrEmpty(timingsAssetPath) || Data.Clip != null) && GUILayout.Button(new GUIContent("Save", "Save current timings to asset"), EditorStyles.toolbarButton))
            {
                SaveAsset();
            }

            EditorGUI.BeginDisabledGroup(Data.Clip == null);
            whisperEditorHelper.DisplayLanguagesPopup();
            var generateTimings = GUILayout.Button(new GUIContent("Generate timings"), EditorStyles.toolbarButton);
            EditorGUI.EndDisabledGroup();
            
            if (Data.Clip != null && GUILayout.Button(new GUIContent("Validate", "Validates timings text"), EditorStyles.toolbarButton))
            {
                timingsValidator.ValidateTimings(Data);
            }

            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(timingsAssetPath == string.Empty);
            AutoSave = GUILayout.Toggle(autoSave, "Auto Save", EditorStyles.toggle);
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck() && (!AutoSave || AutoSave && EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to switch to Auto Save mode? All your unsaved changes will be lost.", "Yes", "Cancel")))
            {
                UpdateDataAsset();
                Init();
            }
            GUILayout.Label("| ", EditorStyles.label);
            curveSize = Mathf.Clamp(curveSize, 1f, curveSize);
            var zoomValue = curveSize.ToString("0.00");
            GUILayout.Label("Zoom: " + zoomValue + "x", EditorStyles.label);
            GUILayout.EndHorizontal();

            if (generateTimings)
            {
                var data = await whisperEditorHelper.GetData(Data.Clip);
                if (data != null)
                {
                    Undo.RecordObject(Data, "Timings Generated");

                    Data.Text = data.Text;
                    Data.Timings.Clear();
                    foreach (var t in data.Timings)
                    {
                        //Deep cloning
                        var newTiming = new Timing(t.StartPosition, t.EndPosition, t.Color, t.Name, t.Text);
                        Data.Timings.Add(newTiming);
                    }
                }
            }
        }

        private void CreateAsset()
        {
            timingsAssetPath = string.Empty;
            Data = CreateInstance<PhraseAsset>();
            Data.hideFlags = HideFlags.HideAndDontSave;
            instance.titleContent = new GUIContent(Title);
        }

        private void LoadAsset(string path)
        {
            if (path != string.Empty)
            {
                var index = path.IndexOf("Assets", StringComparison.Ordinal);
                if (index > 0)
                {
                    path = path.Remove(0, index);
                }
                path = path.Replace(Application.dataPath, "Assets");

                var extension = Path.GetExtension(path);
                if (extension == ".asset")
                {
                    timingsAssetPath = path;
                    var previousAsset = loadedAsset;
                    loadedAsset = AssetDatabase.LoadAssetAtPath<PhraseAsset>(path);
                    if (loadedAsset != null)
                    {
                        UpdateDataAsset();
                        OnLoadData();
                        if (previousAsset != loadedAsset)
                        {
                            Undo.ClearAll();
                        }
                        instance.titleContent = new GUIContent($"{Title} - {Path.GetFileNameWithoutExtension(path)}");
                    }
                    else
                    {
                        Debug.LogWarning("Can't load asset!");
                    }
                }
                else if (extension == ".srt")
                {
                    using (var fileStream = File.OpenRead(path))
                    {
                        var parser = new SrtParser();
                        var items = parser.Parse(fileStream, Encoding.Default);
                        Data.Timings.Clear();
                        var text = new string[items.Count];
                        for (var i = 0; i < items.Count; i++)
                        {
                            var subtitle = items[i];
                            var startTime = subtitle.StartTime / 1000f;
                            var endTime = subtitle.EndTime / 1000f;
                            var lines = string.Join(Environment.NewLine, subtitle.Lines.ToArray());
                            var newTiming = new Timing(startTime, endTime, Timings.DefaultColor, i.ToString(), lines);
                            Data.Timings.Add(newTiming);
                            text[i] = lines;
                        }
                        Data.Text = string.Join(" ", text);
                    }
                }
            }
        }

        private void UpdateDataAsset()
        {
            if (AutoSave)
            {
                Data = loadedAsset;
            }
            else
            {
                Data = CreateInstance<PhraseAsset>();
                Data.hideFlags = HideFlags.HideAndDontSave;
                Data.Clip = loadedAsset.Clip;
                if (!string.IsNullOrEmpty(loadedAsset.Text))
                {
                    Data.Text = loadedAsset.Text.Trim();
                }
                Data.Timings.Clear();
                foreach (var loadedTiming in loadedAsset.Timings) 
                {
                    //Deep cloning
                    var newTiming = new Timing(loadedTiming.StartPosition, loadedTiming.EndPosition, loadedTiming.Color, loadedTiming.Name, loadedTiming.Text);
                    Data.Timings.Add(newTiming);
                }
            }
        }

        private void SaveAsset()
        {
            var asset = CreateInstance<PhraseAsset>();
            asset.Clip = Data.Clip;
            if (!string.IsNullOrEmpty(Data.Text))
            {
                asset.Text = Data.Text.Trim();
            }
            Data.Timings.Sort((x, y) => x.StartPosition.CompareTo(y.StartPosition));
            asset.Timings = Data.Timings;
            if (timingsAssetPath == string.Empty)
            {
                timingsAssetPath = EditorUtility.SaveFilePanelInProject("Asset", "Timings", "asset", "Choose path to save asset");
                if (timingsAssetPath != string.Empty)
                {
                    AssetDatabase.CreateAsset(asset, timingsAssetPath);
                    loadedAsset = asset;
                    instance.titleContent = new GUIContent($"{Title} - {Path.GetFileNameWithoutExtension(timingsAssetPath)}");
                }
            }
            else
            {
                if (loadedAsset != null)
                {
                    EditorUtility.CopySerializedManagedFieldsOnly(asset, loadedAsset);
                    EditorUtility.SetDirty(loadedAsset);
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    Debug.Log("Can't save asset! Loaded asset was destroyed!");
                    timingsAssetPath = string.Empty;
                }
            }
        }

        private void DrawAudioCursor()
        {
            if (audioManager.Source == null)
                return;
            
            var xMin = audioManager.Source.time / Data.Clip.length * curveRect.width + curvePadding.x;
            var audioPositionRect = new Rect(xMin, EditorStyles.toolbar.fixedHeight, 2f, curvePanel.height + EditorStyles.toolbar.fixedHeight);
            EditorGUI.DrawRect(audioPositionRect, audioCursorColor);
            var audioPosition = ExtendedMethods.FormatTimeSpan(TimeSpan.FromSeconds(audioManager.Source.time), true);
            var audioLabelRect = new Rect(xMin + 4f, curvePanel.height - ScrollHeight, 50f, 20f);
            var audioLabelStyle = EditorStyles.miniLabel;
            audioLabelStyle.alignment = TextAnchor.MiddleLeft;
            EditorGUI.LabelField(audioLabelRect, audioPosition, audioLabelStyle);
        }

        private void DrawBottomPanel()
        {
            EditorGUI.BeginDisabledGroup(Data.Clip == null);
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            if (GUILayout.Button(new GUIContent("Add Timing", "Add a new timing to timeline"), EditorStyles.toolbarButton))
            {
                AddTiming(Event.current.mousePosition.x);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(timings.GetSelectedTimingsCount() == 0);
            var postfix = timings.GetSelectedTimingsCount() > 1 ? "s" : string.Empty;
            if (GUILayout.Button(new GUIContent("Remove Timing" + postfix, "Remove selected timing/s from timeline"), EditorStyles.toolbarButton) && timings.GetSelectedTimingsCount() > 0)
            {
                RemoveTiming();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(Data.Clip == null);
            EditorGUI.BeginChangeCheck();
            var iconName = audioManager != null && audioManager.Source != null && audioManager.Source.isPlaying ? "PauseButton" : "PlayButton";
            var playContent = EditorGUIUtility.IconContent(iconName);
            playContent.tooltip = "Play current Audio Clip";
            if (audioManager != null && audioManager.Source != null)
            {
                isPlaying = GUILayout.Toggle(audioManager.Source.isPlaying, playContent, EditorStyles.toolbarButton);
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                if (isPlaying)
                {
                    PlayClip();
                }
                else
                {
                    PauseClip();
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(timings.GetFirstSelectedTiming() == null);
            var playTimingContent = EditorGUIUtility.IconContent("Animation.NextKey");
            playTimingContent.tooltip = "Play selected timing";
            if (GUILayout.Button(playTimingContent, EditorStyles.toolbarButton))
            {
                PlayTiming();
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }

        private void DrawSeparator()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAudioClipField()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("AudioClip:", GUILayout.Width(LabelWidth));
            Undo.RecordObject(Data, "AudioClip Change");
            EditorGUI.BeginChangeCheck();
            Data.Clip = (AudioClip) EditorGUILayout.ObjectField(Data.Clip, typeof(AudioClip), false, GUILayout.Width(position.width - LabelWidth - 12f));
            if (EditorGUI.EndChangeCheck())
            {
                audioManager.SetPosition(0f);
                OnLoadData();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnLoadData()
        {
            curve.LoadData(Data.Clip);
            audioManager.Destroy();
            audioManager.InitSource(Data.Clip);
            if (timings != null)
            {
                timings.Deselect();
            }
            curveSize = 1f;
        }

        private void DrawTextField()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Text:", EditorStyles.label);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            var textAreaStyle = new GUIStyle(GUI.skin.textArea)
            {
                wordWrap = true,
                stretchHeight = true,
                stretchWidth = true,
                richText = true,
            };
            scrollTextAreaPosition = EditorGUILayout.BeginScrollView(scrollTextAreaPosition);
            Undo.RecordObject(Data, "Text Changed");
            Data.Text = EditorGUILayout.TextArea(Data.Text, textAreaStyle, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }

        private void AddTiming(float xPosition)
        {
            var startTime = audioManager.Source.time;
            var endTime = Data.Clip.length;
            var timeOffset = Data.Clip.length / 16f;
            if (audioManager.Source.time + timeOffset <= Data.Clip.length)
            {
                endTime = startTime + timeOffset;
            }
            else
            {
                startTime -= timeOffset;
            }
            timings.AddTiming(startTime, endTime);
            timings.Deselect();
            timings.SelectedLastTimingAdded(xPosition);
        }

        private void RemoveTiming()
        {
            string message;
            if (timings.GetSelectedTimingsCount() > 1)
            {
                message = "Are you sure you want to remove selected timings?";
            }
            else
            {
                if (string.IsNullOrEmpty(timings.GetFirstSelectedTiming().Name))
                {
                    message = "Are you sure you want to remove selected timing?";
                }
                else
                {
                    message = $"Are you sure you want to remove timing \"{timings.GetFirstSelectedTiming().Name}\"?";
                }
            }

            var selectedMany = timings.GetSelectedTimingsCount() > 1 && EditorUtility.DisplayDialog("Confirmation", message, "Yes", "Cancel");
            var selectedOne = timings.GetSelectedTimingsCount() == 1 && EditorUtility.DisplayDialog("Confirmation", message, "Yes", "Cancel");
            
            if (selectedOne)
            {
                Undo.RecordObject(Data, "Remove Timing");
                Data.Timings.Remove(timings.GetFirstSelectedTiming());
            }
            else if (selectedMany)
            {
                Undo.RecordObject(Data, "Remove Timings");
                var selectedTimings = timings.GetSelectedTimings();
                foreach (var timing in selectedTimings)
                {
                    Data.Timings.Remove(timing);
                }
            }
            
            if (selectedMany || selectedOne)
            {
                timings.Deselect();
            }
        }

        private void PlayClip()
        {
            if (audioManager.Source.time >= audioManager.Source.clip.length)
            {
                audioManager.Source.time = 0f;
            }
            audioManager.Source.clip = Data.Clip;
            audioManager.Source.Play();
            isPlaying = true;
        }

        private void PauseClip()
        {
            if (audioManager != null && audioManager.Source != null)
            {
                audioManager.Source.Pause();
            }
            isPlaying = false;
        }

        private void PlayTiming()
        {
            audioPlayingTiming = timings.GetFirstSelectedTiming();
            audioManager.Source.clip = Data.Clip;
            audioManager.Source.time = audioPlayingTiming.StartPosition;
            audioManager.Source.Play();
            isPlaying = true;
            isPlayingTiming = true;
        }
        
        private void RestoreClipPosition(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                audioManager.RestorePosition();
            }
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Repaint();
            }
        }

        private float GetPixelPosition(float pos)
        {
            return pos / Data.Clip.length * curveRect.width + curvePadding.x;
        }

        public void Init(bool reInitialize = false, string assetPath = null)
        {
            instance = GetWindow<TimingsWindow>(Title, false);
            if (whisperEditorHelper == null)
            {
                whisperEditorHelper = new WhisperEditorHelper();
            }
            
            if (!string.IsNullOrEmpty(assetPath))
            {
                timingsAssetPath = assetPath;
            }
            
            if (reInitialize)
            {
                Data = null;
                isDataLoaded = false;
                timingsAssetPath = string.Empty;
            }

            if (audioManager == null || reInitialize)
            {
                if (audioManager != null)
                {
                    PauseClip();
                }
                audioManager = new AudioManager();
            }

            if (curve == null || reInitialize)
            {
                curve = new Curve();
            }

            if (Data == null)
            {
                Data = CreateInstance<PhraseAsset>();
                Data.hideFlags = HideFlags.HideAndDontSave;
                var selection = Selection.activeObject;
                if (selection != null && selection is AudioClip clip)
                {
                    Data.Clip = clip;
                    OnLoadData();
                    isDataLoaded = true;
                }
                else if (selection != null && selection is PhraseAsset)
                {
                    timingsAssetPath = AssetDatabase.GetAssetPath(selection);
                    LoadAsset(timingsAssetPath);
                    timings?.Deselect();
                    isDataLoaded = true;
                }
                else if (!string.IsNullOrEmpty(assetPath))
                {
                    timingsAssetPath = assetPath;
                    LoadAsset(timingsAssetPath);
                    timings?.Deselect();
                    isDataLoaded = true;
                }
            }

            if (audioManager.Source == null || reInitialize)
            {
                audioManager.InitSource(Data.Clip);
            }

            if (timeline == null || reInitialize)
            {
                timeline = new Timeline();
            }

            if (timings == null || reInitialize)
            {
                timings = new Timings();
            }

            if (timingsValidator == null || reInitialize)
            {
                timingsValidator = new TimingsValidator();
            }
            
            if (input == null || reInitialize)
            {
                input = new Input
                {
                    OnMouseWheel = OnMouseWheel,
                    OnMouseDown = OnMouseDown,
                    OnMouseDrag = OnMouseDrag,
                    OnMouseUp = timings.OnMouseUp,
                    OnRepaint = timings.DrawSelectionRect,
                };
                input.OnKeyDown -= timings.OnKeyDown;
                input.OnKeyDown += timings.OnKeyDown;
                input.OnKeyDown -= OnKeyDown;
                input.OnKeyDown += OnKeyDown;
            }

            if (!isDataLoaded && reInitialize)
            {
                isDataLoaded = true;
                LoadAsset(timingsAssetPath);
            }

            autoSave = EditorPrefs.GetBool("ATS_AutoSave", false);

            Undo.undoRedoPerformed -= Repaint;
            Undo.undoRedoPerformed += Repaint;
            EditorApplication.playModeStateChanged -= RestoreClipPosition;
            EditorApplication.playModeStateChanged += RestoreClipPosition;
        }

        private void OnKeyDown(int controlId)
        {
            if (Event.current != null && Event.current.isKey)
            {
                if (doubleClickRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.keyCode == KeyCode.Backspace || Event.current.keyCode == KeyCode.Delete)
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use();

                        if (timings.GetSelectedTimingsCount() > 0)
                        {
                            RemoveTiming();
                        }
                    }
                    else if (Event.current.keyCode == KeyCode.A && (Event.current.command || Event.current.alt))
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use();

                        timings.SelectAllTimings();
                    }
                }
            }
        }

        public Rect GetPixelRect(float pos)
        {
            var pixelPosition = GetPixelPosition(pos);
            return new Rect(pixelPosition, EditorStyles.toolbar.fixedHeight, 3f, curvePanel.height);
        }

        public Rect GetCurveRect()
        {
            return curveRect;
        }

        public Rect GetTimingsRect()
        {
            return timingsRect;
        }

        public float GetPanelHeight()
        {
            return curvePanel.height;
        }
    }
}