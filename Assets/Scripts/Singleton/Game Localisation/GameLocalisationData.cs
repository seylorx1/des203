using UnityEngine;

public class GameLocalisationData : ScriptableObject, ISingletonData {
    public bool Foldout { get; set; } = false;
    public LanguageLocalisationStruct[] Languages = new LanguageLocalisationStruct[0];

}

[System.Serializable]
public class LanguageLocalisationStruct {
    public string LanguageName;
    public string GameName;

    [System.Serializable]
    public struct MenuStrings {
        public string Play;
        public string Quit;
    }
    public MenuStrings Menu;
}
