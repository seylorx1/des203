using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingletonMonoBehaviour<InputManager> {
    protected override void Awake() {
        base.Awake();
    }

    private void Start() {
        InputManagerData data = (InputManagerData)SingletonBaseRef.Data;

        data.movement.Enable();
        data.look.Enable();
        data.jump.Enable();
        data.snipModeToggle.Enable();
        data.leftCrabClaw.Enable();
        data.rightCrabClaw.Enable();
    }
}
