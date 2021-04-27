using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Helper {
    public static Vector3 Invert(this Vector3 i) {
        return new Vector3(1.0f / i.x, 1.0f / i.y, 1.0f / i.z);
    }
}
