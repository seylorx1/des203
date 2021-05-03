using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Helper {
    public static Vector3 Invert(this Vector3 i) {
        return new Vector3(1.0f / i.x, 1.0f / i.y, 1.0f / i.z);
    }

    public static Vector3 Round(this Vector3 i, uint decimalPlaces) {
        float factor = Mathf.Pow(10.0f, (float)decimalPlaces);
        return new Vector3(
            Mathf.Round(i.x * factor) / factor,
            Mathf.Round(i.y * factor) / factor,
            Mathf.Round(i.z * factor) / factor
            );
    }
}
