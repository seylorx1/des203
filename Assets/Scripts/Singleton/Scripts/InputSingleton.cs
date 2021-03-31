using UnityEditor;
using UnityEngine.InputSystem;

public class InputSingleton : SingletonScriptableObject {
    public InputAction movement;
    public InputAction look;
    public InputAction jump;
    public InputAction snipModeToggle;
    public InputAction leftCrabClaw;
    public InputAction rightCrabClaw;
    public InputAction pause;

    public override void OnStart() {
        movement.Enable();
        look.Enable();
        jump.Enable();
        snipModeToggle.Enable();
        leftCrabClaw.Enable();
        rightCrabClaw.Enable();
        pause.Enable();
    }

    [MenuItem("Assets/Create/Singleton/InputSingleton")]
    public static void CreateAsset() {
        ScriptableObjectHelper.CreateAsset<InputSingleton>();
    }
}
