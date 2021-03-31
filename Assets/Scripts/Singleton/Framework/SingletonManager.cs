using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonManager : MonoBehaviour {
    public static SingletonManager Instance = null;

    public List<SingletonScriptableObject> singletonScriptableObjects = new List<SingletonScriptableObject>();

    public T GetSingleton<T>() where T : SingletonScriptableObject {
        foreach(SingletonScriptableObject singletonSO in singletonScriptableObjects) {
            if(typeof(T) == singletonSO.GetType()) {
                return (T)singletonSO;
            }
        }

        return null;
    }

    private void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }

        singletonScriptableObjects.ForEach(singletonSO => {
            singletonSO.OnAwake();
        });
    }

    private void Start() {
        singletonScriptableObjects.ForEach(singletonSO => {
            singletonSO.OnStart();
        });
    }

    private void Update() {
        singletonScriptableObjects.ForEach(singletonSO => {
            singletonSO.OnUpdate();
        });
    }
}
