using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

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
                        ((ISingletonData)singletonBase.Data).Foldout = EditorGUILayout.Foldout(
                            ((ISingletonData)singletonBase.Data).Foldout,
                            singletonBase.SingletonType.Type.Name);
                    }
                    else {
                        EditorGUILayout.LabelField(singletonBase.GetType().Name + " (Error)");
                        Debug.LogError($"Singleton Data class {singletonBase.DataType.Type.Name} does not implement ISingletonData!");
                    }

                    Rect remove_buttonArea = GUILayoutUtility.GetRect(25.0f, 25.0f);
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

                        do {
                            EditorGUILayout.PropertyField(_iterator);
                        }
                        while (_iterator.NextVisible(false));
                        singletonSerializedData.ApplyModifiedProperties();

                        ///
                        singletonBase.UpdateRawFromData();

                        EditorGUI.indentLevel--;
                    }

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
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 25.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag and Drop SingletonBase Class");

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
