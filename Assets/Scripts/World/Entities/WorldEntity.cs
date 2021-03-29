using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEntity : MonoBehaviour {
    public bool verbose = false;

    [SerializeField]
    private int
        health = 10,
        healthMax = 10;

    private Material crackMaterialInstance;

    protected virtual void Awake() {
        crackMaterialInstance = GetComponent<MeshRenderer>().material;

        if (!crackMaterialInstance.HasProperty("_CrackAmount")) {
            crackMaterialInstance = null;
        }
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

        if(crackMaterialInstance != null) {
            crackMaterialInstance.SetFloat("_CrackAmount", Mathf.Clamp01(1.0f - HealthAsPercent()));
        }

        //Handle death, if applicable.
        if (IsDead()) {
            Die();
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
}
