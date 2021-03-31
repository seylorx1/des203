using UnityEngine;

public abstract class SingletonScriptableObject : ScriptableObject {
    public virtual void OnAwake() {}
    public virtual void OnStart() {}
    public virtual void OnUpdate() {}
}
