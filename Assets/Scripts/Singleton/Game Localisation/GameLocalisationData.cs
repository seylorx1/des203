using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLocalisationData : ScriptableObject, ISingletonData {
    public bool Foldout { get; set; } = false;
    public LanguageLocalisationStruct[] Languages = new LanguageLocalisationStruct[0];

}

[System.Serializable]
public class LanguageLocalisationStruct {
    public string languageName;
}
