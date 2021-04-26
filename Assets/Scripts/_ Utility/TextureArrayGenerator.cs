using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class TextureArrayGenerator : EditorWindow {
    public static List<Texture2D> frames = new List<Texture2D>();

    private static bool sequenceFoldout = true;
    private static string sequenceName = "";
    private static int sequencePreviewFrame = 0;
    private static double sequencePreviewSpeed = 30.0d;
    private static bool sequenceErrors = false;

    private static int lastTimeSinceStartup = -1;
    private static Vector2 scrollPos;

    private static GUIStyle crabertayStyle;

    [MenuItem("Assets/Create/2DTextureArray")]
    public static void Create2DTextureArray() {
        GetWindow<TextureArrayGenerator>(false, "2D Texture Array Generator", true);
    }

    void OnEnable() {
        lastTimeSinceStartup = (int)EditorApplication.timeSinceStartup;
        sequenceName = "New 2DTextureArray";
        frames.Clear();
    }

    void OnGUI() {
        minSize = new Vector2(350, 300);

        int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);

        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
        if (Event.current.type == EventType.DragExited) {
            foreach (Object obj in DragAndDrop.objectReferences) {
                if (obj is Texture2D) {
                    frames.Add((Texture2D)obj);
                    sequenceFoldout = true;
                }
            }
        }
        if (EditorGUIUtility.GetObjectPickerControlID() == controlID) {
            if (Event.current.commandName == "ObjectSelectorClosed") {
                frames.Add((Texture2D)EditorGUIUtility.GetObjectPickerObject());
            }
        }

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Sequence Name", GUILayout.Width(100));
        sequenceName = EditorGUILayout.TextField(sequenceName);
        EditorGUILayout.Space(10);

        GUI.enabled = frames.Count > 0 && !sequenceErrors;
        if (GUILayout.Button(new GUIContent("Create", frames.Count == 0 ? "Not enough images in sequence!" : sequenceErrors ? "Errors in sequence!" : ""), GUILayout.Width(64))) {
            CreateAsset(sequenceName);
            Close();
        }
        EditorGUILayout.EndHorizontal();

        TryDrawSequence();

        GUI.enabled = true;

        EditorGUILayout.Separator();
        sequenceFoldout = EditorGUILayout.Foldout(sequenceFoldout, $"Sequence ({frames.Count})");
        if (sequenceFoldout) {
            if (GUILayout.Button("Add Image to Sequence", GUILayout.Height(32))) {
                EditorGUIUtility.ShowObjectPicker<Texture2D>(null, false, "", controlID);
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            for (int i = 0; i < frames.Count; i++) {
                sequenceErrors = false;

                Texture2D frame = frames[i];
                if (frame != null) {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.Space(34);

                    GUIStyle errorStyle = new GUIStyle(GUI.skin.label);
                    errorStyle.normal.textColor = Color.red;


                    string frameError = "";

                    //Check if frame is readable.
                    if (!frame.isReadable)
                        frameError = "Error: Read/Write is not enabled!";

                    //Check frame is not crunch format.
                    switch (frame.format) {
                        case TextureFormat.RGBA32:
                        case TextureFormat.ARGB32:
                        case TextureFormat.BGRA32:
                        case TextureFormat.RFloat:
                        case TextureFormat.RG32:
                            break;
                        default:
                            frameError = "Error: Invalid format!";
                            break;
                    }

                    GUILayout.Label(new GUIContent(frame.name, frameError), frameError != "" ? errorStyle : GUI.skin.label, GUILayout.Height(34));

                    sequenceErrors = frameError != "" ? true : sequenceErrors;


                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    EditorGUI.DrawTextureTransparent(new Rect(0, lastRect.y, 32, 32), frame);


                    GUI.enabled = i > 0;
                    if (GUILayout.Button("▲", GUILayout.Height(32), GUILayout.Width(25))) {
                        Texture2D swap = frames[i - 1];
                        frames[i - 1] = frame;
                        frames[i] = swap;
                        break;
                    }

                    GUI.enabled = i < frames.Count - 1;
                    if (GUILayout.Button("▼", GUILayout.Height(32), GUILayout.Width(25))) {
                        Texture2D swap = frames[i + 1];
                        frames[i + 1] = frame;
                        frames[i] = swap;
                        break;
                    }

                    GUI.enabled = true;
                    if (GUILayout.Button("Remove", GUILayout.Height(32), GUILayout.Width(64))) {
                        frames.RemoveAt(i);
                        break;
                    }
                }
                else {
                    frames.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        if (frames.Count > 0) {
            GUILayout.Space(5);
            if (GUILayout.Button($"Clear {frames.Count} Item{(frames.Count > 1 ? "s" : "")}")) {
                frames.Clear();
            }
        }


        if (frames.Count == 0 || !sequenceFoldout) {
            GUIContent crabertayGUIContent = new GUIContent("Created by Team Crabertay ❤", "Click me! \\(￣︶￣*\\)");

            if (crabertayStyle == null) {
                crabertayStyle = new GUIStyle(GUI.skin.label);
                crabertayStyle.hover.textColor = new Color(1.0f, 0.75f, 0.8f, 1.0f);
                crabertayStyle.fontStyle = FontStyle.Bold;
                crabertayStyle.fontSize = 10;
                crabertayStyle.alignment = TextAnchor.LowerCenter;
            }

            Rect crabertayLabelRect = GUILayoutUtility.GetRect(crabertayGUIContent, crabertayStyle);
            if (GUI.Button(new Rect(0, position.height - crabertayLabelRect.height - 5, position.width, crabertayLabelRect.height), crabertayGUIContent, crabertayStyle)) {
                Application.OpenURL("https://www.twitter.com/crabertay");
            }

            Repaint();
        }
    }

    private void TryDrawSequence() {
        if (frames.Count > 0) {
            int imgPad = 6;

            int currentTimeSinceStartup = (int)(EditorApplication.timeSinceStartup * sequencePreviewSpeed);
            if (lastTimeSinceStartup != currentTimeSinceStartup) {
                sequencePreviewFrame = sequencePreviewFrame < frames.Count - 1 ? sequencePreviewFrame + 1 : 0;
                lastTimeSinceStartup = currentTimeSinceStartup;
            }

            EditorGUILayout.BeginHorizontal();
            Rect r = GUILayoutUtility.GetRect(64, 64);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space(8);

            GUI.enabled = false;

            GUILayout.Label($"Frame {sequencePreviewFrame + 1} of {frames.Count}.");

            GUILayout.Label($"Previewing sequence at {sequencePreviewSpeed} fps.");

            EditorGUILayout.BeginHorizontal();

            GUIStyle changeSpeedButtonStyle = new GUIStyle(GUI.skin.button);
            changeSpeedButtonStyle.fontSize = 10;

            GUI.enabled = sequencePreviewSpeed > 10.0d;
            if (GUILayout.Button("-10", changeSpeedButtonStyle, GUILayout.Width(32))) { sequencePreviewSpeed -= 10.0d; }

            GUI.enabled = sequencePreviewSpeed > 1.0d;
            if (GUILayout.Button("-1", changeSpeedButtonStyle, GUILayout.Width(32))) { sequencePreviewSpeed -= 1.0d; }

            GUI.enabled = sequencePreviewSpeed <= 120.0d - 1;
            if (GUILayout.Button("+1", changeSpeedButtonStyle, GUILayout.Width(32))) { sequencePreviewSpeed += 1.0d; }

            GUI.enabled = sequencePreviewSpeed <= 120.0d - 10;
            if (GUILayout.Button("+10", changeSpeedButtonStyle, GUILayout.Width(32))) { sequencePreviewSpeed += 10.0d; }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUI.DrawTextureTransparent(new Rect(5, r.y + imgPad / 2, 64, 64), frames[sequencePreviewFrame]);
            Repaint();
        }
        else {
            sequencePreviewFrame = 0;
        }
    }

    private void CreateAsset(string sequenceName) {
        Texture2DArray tex2Darr = new Texture2DArray(
            frames[0].width,
            frames[0].height,
            frames.Count,
            frames[0].format,
            frames[0].mipmapCount > 1);

        for (int i = 0; i < frames.Count; i++) {
            tex2Darr.SetPixels32(frames[i].GetPixels32(), i);
        }
        ObjectHelper.CreateAsset(tex2Darr, sequenceName != null && sequenceName.Length > 0 ? sequenceName : "New 2DTextureArray");
    }
}
#endif
