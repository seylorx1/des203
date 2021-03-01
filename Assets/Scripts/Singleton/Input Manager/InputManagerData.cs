using UnityEngine;
using UnityEngine.InputSystem;

public class InputManagerData : ScriptableObject, ISingletonData {
    public bool Foldout { get; set; } = false;

    [SerializeField] public InputAction movement;
    [SerializeField] public InputAction look;
    [SerializeField] public InputAction jump;
    [SerializeField] public InputAction snipModeToggle;
    [SerializeField] public InputAction leftCrabClaw;
    [SerializeField] public InputAction rightCrabClaw;

}
