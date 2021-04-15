using UnityEditor;
using UnityEngine.InputSystem;

public class InputSingleton : SingletonScriptableObject {
    public InputAction movement;
    public InputAction look;
    public InputAction jump;
    public InputAction snipModeToggle;
    public InputAction leftCrabClaw;
    public InputAction rightCrabClaw;
    public InputAction pivotLeftCrabClaw;
    public InputAction pivotRightCrabClaw;
    public InputAction pause;
    public InputAction camToggle;

    public override void OnAwake() {
        movement.Enable();
        look.Enable();
        jump.Enable();
        snipModeToggle.Enable();
        leftCrabClaw.Enable();
        rightCrabClaw.Enable();
        pivotLeftCrabClaw.Enable();
        pivotRightCrabClaw.Enable();
        pause.Enable();
        camToggle.Enable();
    }

    [MenuItem("Assets/Create/Singleton/InputSingleton")]
    public static void CreateAsset() {
        ScriptableObjectHelper.CreateAsset<InputSingleton>();
    }
}
