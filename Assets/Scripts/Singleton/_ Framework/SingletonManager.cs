using System;
using System.Collections.Generic;
using UnityEngine;
using TypeReferences;

public class SingletonManager : MonoBehaviour {
    [SerializeField] private bool _foldoutSingletonClasses;
    public List<SingletonBase> SingletonBases;

    private void Awake() {
        foreach (SingletonBase singletonBase in SingletonBases) {
            GameObject go = new GameObject(singletonBase.SingletonType.Type.Name);
            ((ISingletonInstance)go.AddComponent(singletonBase.SingletonType.Type)).SingletonBaseRef = singletonBase;
            go.transform.parent = transform;
        }
        DontDestroyOnLoad(gameObject);
    }
}