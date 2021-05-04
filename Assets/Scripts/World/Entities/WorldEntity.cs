using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEntity : MonoBehaviour, IInteractable {
    public bool verbose = false;
    public string gibID = "";

    protected GibManagerSingleton.GibBase gib;

    [SerializeField]
    private int
        health = 10,
        healthMax = 10;

    private List<Material> crackMaterialInstances = new List<Material>();

    private MeshFilter meshFilter;

    protected virtual void Awake() {
        if (gibID == "") {
            gibID = "metal"; //Default to the metal gib.
        }

        meshFilter = GetComponentInChildren<MeshFilter>();

        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer mr in meshRenderers) {
            Material m = mr.material;
            if (m.HasProperty("_CrackAmount")) {
                crackMaterialInstances.Add(m);
            }
        }
    }

    protected virtual void Start() {
        gib = SingletonManager.Instance.GetSingleton<GibManagerSingleton>().GetGib(gibID);
    }

    public float HealthAsPercent() {
        return ((float)health) / ((float)healthMax);
    }

    public bool Damage(int damageAmount) {
        //Throw an error if negative damage is sent.
        if (damageAmount <= 0) {
            Debug.LogError($"WorldEntity just received {damageAmount} damage!", gameObject);
            return false;
        }

        //Take health.
        health -= damageAmount;

        if (verbose) {
            Debug.Log($"{transform.name} just took {damageAmount} damage! Current health: {health}.");
        }

        if (crackMaterialInstances.Count > 0) {
            foreach (Material m in crackMaterialInstances) {
                m.SetFloat("_CrackAmount", Mathf.Clamp01(1.0f - HealthAsPercent()));
            }
        }


        //Handle death, if applicable.
        if (IsDead()) {
            gib?.SpawnGibExplosion(meshFilter, transform.position);
            Die();
        }
        else {
            gib?.SpawnParticlesAtPosition(transform.position);
        }
        return true;
    }

    public virtual bool IsDead() {
        return health <= 0;
    }

    /// <summary>
    /// Called when the entity dies. Can be overriden.
    /// </summary>
    public virtual void Die() {
        Destroy(gameObject);
    }

    public virtual IInteractable.Type GetInteractableType() {
        return IInteractable.Type.Breakable;
    }
}
