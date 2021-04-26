using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public static class ObjectHelper {
    public static void CreateAsset<T>() where T : ScriptableObject { 
        T asset = ScriptableObject.CreateInstance<T>();
        CreateAsset(asset);
    }

    public static void CreateAsset<T>(T obj) where T : Object{
        CreateAsset(obj, "New " + typeof(T).ToString());
    }

    public static void CreateAsset<T>(T obj, string name) where T : Object {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "") {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "") {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".asset");
        AssetDatabase.CreateAsset(obj, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = obj;
    }
}
#endif