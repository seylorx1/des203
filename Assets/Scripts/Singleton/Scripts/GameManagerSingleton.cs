using UnityEngine;
using UnityEditor;

public class GameManagerSingleton : SingletonScriptableObject {

    public string name = "example";

    [MenuItem("Assets/Create/Singleton/GameManagerSingleton")]
    public static void CreateAsset() {
        ScriptableObjectHelper.CreateAsset<GameManagerSingleton>();
    }
}
