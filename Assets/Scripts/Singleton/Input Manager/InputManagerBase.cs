using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerBase : SingletonBase {
    public InputManagerBase() : base(typeof(InputManager), typeof(InputManagerData)) { }
}
