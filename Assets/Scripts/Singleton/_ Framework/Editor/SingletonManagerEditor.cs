using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditorInternal;

[CustomEditor(typeof(SingletonManager))]
public class SingletonManagerEditor : Editor {
    SerializedProperty _foldoutSingletonClasses;
    SingletonManager t;

    void OnEnable() {
        _foldoutSingletonClasses = serializedObject.FindProperty("_foldoutSingletonClasses");
        t = target as SingletonManager;

        if (t.SingletonBases != null) {
            if (t.gameObject.scene.rootCount > 0) {
                //object is not a prefab.
                t.SingletonBases.ForEach((pair) => {
                    pair.ResetData(true);
                    pair.UpdateDataFromRaw();
                });
            }
        } else {
            t.SingletonBases = new List<SingletonBase>();
        }
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        Event evt = Event.current;

        #region ClassTypeReference List Foldout

        //Create foldout and update its value.
        _foldoutSingletonClasses.boolValue = EditorGUILayout.Foldout(
            _foldoutSingletonClasses.boolValue,
            $"Singletons ({(t.SingletonBases != null ? t.SingletonBases.Count.ToString() : "null")})");

        //When singletons foldout is enabled...
        if (_foldoutSingletonClasses.boolValue) {
            EditorGUI.indentLevel++;

            //Check to see if t.SingletonPairs is valid.
            if (t.SingletonBases != null && t.SingletonBases.Count > 0) {

                //Create a reference to the pair to remove.
                //(The user can only click on one pair at a time, so no need for a list.)
                SingletonBase markRemovePair = null;

                //Iterate through each pair.
                t.SingletonBases.ForEach((singletonBase) => {

                    EditorGUILayout.BeginHorizontal();

                    if(singletonBase.Data == null) {
                        Debug.Log("Data not loaded!");
                    }

                    //If data implements ISingletonData, create a dropdown. Otherwise, log an error!
                    if (typeof(ISingletonData).IsAssignableFrom(singletonBase.DataType.Type)) {

                        //Create foldout and update foldout variable.
                        GUIStyle singletonBaseFoldoutLabelStyle = new GUIStyle(EditorStyles.foldoutHeader);
                        GUIContent singletonBaseFoldoutContent = new GUIContent(singletonBase.SingletonType.Type.Name + " ↪");
                        Rect singletonBaseFoldoutArea = GUILayoutUtility.GetRect(singletonBaseFoldoutContent, singletonBaseFoldoutLabelStyle);
                        ((ISingletonData)singletonBase.Data).Foldout = EditorGUI.Foldout(
                            singletonBaseFoldoutArea,
                            ((ISingletonData)singletonBase.Data).Foldout,
                            singletonBaseFoldoutContent,
                            singletonBaseFoldoutLabelStyle);

                        switch(evt.type) {
                            case EventType.MouseDown:
                                if(singletonBaseFoldoutArea.Contains(evt.mousePosition)) {

                                    string[] guids = AssetDatabase.FindAssets("t:script " + singletonBase.SingletonType.Type.Name);
                                    if(guids != null) {
                                        foreach(string guid in guids) {
                                            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                                            if(Path.GetFileNameWithoutExtension(assetPath) == singletonBase.SingletonType.Type.Name) {
                                                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)));
                                                break;
                                            }
                                        }
                                    }

                                }
                                break;
                        }
                    }
                    else {
                        EditorGUILayout.LabelField(singletonBase.GetType().Name + " (Error)");
                        Debug.LogError($"Singleton Data class {singletonBase.DataType.Type.Name} does not implement ISingletonData!");
                    }

                    Rect remove_buttonArea = GUILayoutUtility.GetRect(75.0f, 30.0f, GUILayout.MaxWidth(75.0f));
                    if (GUI.Button(remove_buttonArea, "Remove")) {
                        markRemovePair = singletonBase;
                    }
                    EditorGUILayout.EndHorizontal();

                    //If data exists and foldout is enabled
                    if (((ISingletonData)singletonBase.Data).Foldout) {
                        EditorGUI.indentLevel++;

                        SerializedObject singletonSerializedData = new SerializedObject(singletonBase.Data);
                        SerializedProperty _iterator = singletonSerializedData.GetIterator();
                        _iterator.NextVisible(true);
                        _iterator.NextVisible(true);

                        SerializedProperty lastArrayProperty = null;
                        int arrayLeft = 0;

                        //Used to remove an array item after iteration.
                        KeyValuePair<SerializedProperty, int> markRemoveArrayElement = new KeyValuePair<SerializedProperty, int>();

                        do {

                            if (_iterator.isArray) {
                                EditorGUILayout.Space();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField(_iterator.displayName + $" ({_iterator.arraySize})", new GUIStyle(EditorStyles.boldLabel));
                                
                                Rect add_arrayElementButton = GUILayoutUtility.GetRect(75.0f, 20.0f, GUILayout.MaxWidth(75.0f));
                                if (GUI.Button(add_arrayElementButton, "Add")) {
                                    _iterator.InsertArrayElementAtIndex(_iterator.arraySize);
                                }

                                lastArrayProperty = _iterator.Copy();
                                arrayLeft = _iterator.arraySize;

                                EditorGUILayout.EndHorizontal();

                                EditorGUI.indentLevel++;

                                //Skip over 'size' parameter.
                                _iterator.NextVisible(true);
                            }
                            else {

                                if (arrayLeft > 0) {
                                    //Inside of array.
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.PropertyField(_iterator);

                                    Rect remove_arrayElementButton = GUILayoutUtility.GetRect(75.0f, 20.0f, GUILayout.MaxWidth(75.0f));
                                    if(GUI.Button(remove_arrayElementButton, "Remove")) {
                                        markRemoveArrayElement = new KeyValuePair<SerializedProperty, int>(lastArrayProperty.Copy(), lastArrayProperty.arraySize - arrayLeft);
                                    }
                                    EditorGUILayout.EndHorizontal();

                                    arrayLeft--;
                                    if(arrayLeft == 0) {
                                        EditorGUI.indentLevel--;
                                    }
                                }
                                else {
                                    EditorGUILayout.PropertyField(_iterator);

                                }
                            }
                        }
                        while (_iterator.NextVisible(_iterator.isArray));

                        //Remove array item.
                        if(markRemoveArrayElement.Key != null) {
                            markRemoveArrayElement.Key.DeleteArrayElementAtIndex(markRemoveArrayElement.Value);
                        }

                        singletonSerializedData.ApplyModifiedProperties();

                        ///
                        singletonBase.UpdateRawFromData();

                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.Space();
                });

                //Remove the marked pair from the list.
                if (markRemovePair != null) {
                    t.SingletonBases.Remove(markRemovePair);
                }
            }
            else {
                GUIStyle italicLabelStyle = new GUIStyle(GUI.skin.label);
                italicLabelStyle.fontStyle = FontStyle.Italic;
                EditorGUILayout.LabelField("No singletons in list.", italicLabelStyle);
            }
            EditorGUI.indentLevel--;
        }
        #endregion

        EditorGUILayout.Space();

        #region Drag and Drop Singleton
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 25.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, $"Drag and Drop {typeof(SingletonBase).Name} Script");

        switch (evt.type) {
            case EventType.DragUpdated:
            case EventType.DragPerform:

                //If drag and drop operation is updated or performed.

                //If the mouse isn't in the correct area, break out of drag and drop.
                if (!dropArea.Contains(evt.mousePosition)) {
                    return;
                }


                //Set the visual mode to link
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;

                if (evt.type == EventType.DragPerform) {
                    DragAndDrop.AcceptDrag();

                    //Iterate through every dragged object.
                    foreach (Object dragged_object in DragAndDrop.objectReferences) {

                        //Only care if the object is a monoscript.
                        if (dragged_object.GetType() == typeof(MonoScript)) {

                            //Get the class of the object.
                            System.Type scriptType = ((MonoScript)dragged_object).GetClass();

                            //Check if the object script is a subclass of SingletonBase.
                            if (scriptType.IsSubclassOf(typeof(SingletonBase))) {

                                //Create a new instance of SingletonBase.
                                SingletonBase singletonBaseInstance = (SingletonBase)System.Activator.CreateInstance(scriptType);
                                singletonBaseInstance.UpdateRawFromData();

                                //If the array doesn't exist, create it.
                                if (t.SingletonBases != null) {
                                    bool draggedObjectInvalid = false;
                                    t.SingletonBases.ForEach((singletonBase) => {
                                        if(singletonBase.SingletonType.Type == singletonBaseInstance.SingletonType.Type) {
                                            //No duplicates allowed!
                                            draggedObjectInvalid = true;
                                        }
                                    });
                                    if (draggedObjectInvalid) {
                                        continue;
                                    }
                                }


                                //Add a new instance of singleton pairs to the array.
                                t.SingletonBases.Add(singletonBaseInstance);

                                //Foldout the singleton list.
                                _foldoutSingletonClasses.boolValue = true;
                            }
                        }
                    }
                }
                break;
        }
        #endregion

        serializedObject.ApplyModifiedProperties();
    }
}
