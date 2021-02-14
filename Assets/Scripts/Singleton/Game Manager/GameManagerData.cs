using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerData : ScriptableObject, ISingletonData {

    public bool Foldout { get; set; } = false;

    public string hello = "Hello world!";
    public Vector3 v3 = new Vector3(1.0f, 2.0f, 3.0f);

}