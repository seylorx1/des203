using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour, ISingletonInstance
    where T : MonoBehaviour {

    public static SingletonMonoBehaviour<T> Instance {
        get;
        private set;
    }

    public SingletonBase SingletonBaseRef { get; set; } = null;

    protected virtual void Awake() {
        if(Instance == null) {
            //Set the instance.
            Instance = this;
        }
        else {
            //Destroy, as this is a duplicate!
            Debug.LogWarning("Duplicate Singleton of type " + typeof(T) + " found! Destroying...");
            Destroy(gameObject);
        }
    }
}

