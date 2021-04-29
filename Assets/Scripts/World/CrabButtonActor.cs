using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CrabButtonActor : MonoBehaviour {
    public abstract void Pressed(Collider other);
}
