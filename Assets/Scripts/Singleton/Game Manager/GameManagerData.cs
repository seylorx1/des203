using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerData : ScriptableObject, ISingletonData {
    public bool Foldout { get; set; } = false;

    public enum GamePhase {
        Title,
        MainMenu,
        Game,
        GameResults
    }
    public GamePhase CurrentPhase = GamePhase.MainMenu;

}