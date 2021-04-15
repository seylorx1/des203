using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;

public class GibManagerSingleton : SingletonScriptableObject {

    [System.Serializable]
    public class GibBase {
        public Color gibColor;
        public Material particleSystemMaterial;
        public Preset particleSystemPreset;

        private GameObject cachedParticleInstance = null;

        //Quite laggy...
        public void CacheParticleSystem() {
            //Create new gameobject and assign a new particle system
            cachedParticleInstance = new GameObject("Cached Particle System Instance"); //Create the gameobject
            ParticleSystem ps = (ParticleSystem)cachedParticleInstance.AddComponent(typeof(ParticleSystem)); //Add a particle system

            //Apply preset
            particleSystemPreset.ApplyTo(ps);

            //Apply main module properties
            ParticleSystem.MainModule ps_mm = ps.main;
            ps_mm.startColor = gibColor;

            ParticleSystemRenderer ps_r = cachedParticleInstance.GetComponent<ParticleSystemRenderer>();

            //Apply material
            ps_r.material = particleSystemMaterial;

            cachedParticleInstance.SetActive(false);
        }

        public GameObject SpawnGibAtPosition(Vector3 worldPos) {
            GameObject ps_go = Instantiate(cachedParticleInstance);
            ps_go.transform.position = worldPos;
            ps_go.name = "Particle System Instance";
            ps_go.SetActive(true);

            return ps_go;
        }
    }

    [System.Serializable]
    public class GibCustom : GibBase {
        [Tooltip("The identifier used to search for gibs."), SerializeField]
        private string id = "gib";

        public string GetID() {
            return id;
        }
    }

    [Tooltip("If an element of a gib is unavailable or empty, default to elements from this gib.")]
    public GibBase defaultGib;
    public List<GibCustom> gibs = new List<GibCustom>();

    public GibCustom GetGib(string id) {
        foreach (GibCustom gib in gibs) {
            if (gib.GetID() == id) {
                return gib;
            }
        }
        Debug.LogWarning($"Cannot find gib with id \"{id}\".");
        return null;
    }

    [MenuItem("Assets/Create/Singleton/GibManagerSingleton")]
    public static void CreateAsset() {
        ScriptableObjectHelper.CreateAsset<GibManagerSingleton>();
    }

    public override void OnAwake() {
        //Populate missing attributes
        foreach (GibBase gib in gibs) {
            //Check to see if any elements are unavailable
            if (gib.particleSystemMaterial == null) {
                gib.particleSystemMaterial = defaultGib.particleSystemMaterial;
            }

            if (gib.particleSystemPreset == null) {
                gib.particleSystemPreset = defaultGib.particleSystemPreset;
            }

            gib.CacheParticleSystem();
        }
    }
}