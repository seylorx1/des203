using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;

public class GibManagerSingleton : SingletonScriptableObject {

    public GameObject[] gibInstanceGameObjects;
    private List<KeyValuePair<int, float>> gibSizes;

    [System.Serializable]
    public class GibBase {

        public Color gibColor;
        public Material particleSystemMaterial;
        public Preset particleSystemPreset;

        private GameObject cachedParticleInstance = null;

        public GibManagerSingleton GibManager { private get; set; }

        //Quite laggy...
        public void CacheParticleSystem(Transform parentTransform) {
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

            cachedParticleInstance.transform.parent = parentTransform;
            cachedParticleInstance.SetActive(false);
        }

        public GameObject SpawnParticlesAtPosition(Vector3 worldPos) {
            GameObject ps_go = Instantiate(cachedParticleInstance);
            ps_go.transform.position = worldPos;
            ps_go.name = "Particle System Instance";
            ps_go.SetActive(true);

            return ps_go;
        }

        public GameObject[] SpawnGibExplosion(MeshFilter meshFilter, Vector3 worldPos) {
            //Max gib size should be, at the most, quarter that of the object.
            float maxSize =
                meshFilter.mesh.bounds.size.magnitude *
                meshFilter.transform.localScale.magnitude *
                0.25f;

            GameObject[] spawnedGameObjects = new GameObject[5];

            for (int i = 3; i < GibManager.gibSizes.Count; i++) {
                if (GibManager.gibSizes[i].Value > maxSize || i == GibManager.gibSizes.Count-1) {
                    for(int j = i-3; j <= i; j++) {
                        //Iterate over the previous 4
                        int spawnedGibIndex = j - i + 3;

                        //Spawn gibs. 
                        spawnedGameObjects[spawnedGibIndex] = Instantiate(GibManager.gibInstanceGameObjects[GibManager.gibSizes[j].Key]);
                        spawnedGameObjects[spawnedGibIndex].transform.position = worldPos;
                        spawnedGameObjects[spawnedGibIndex].transform.rotation = Random.rotation;
                        spawnedGameObjects[spawnedGibIndex].GetComponent<MeshRenderer>().material.color = gibColor;

                        //Force is handled by scripts on gibs...
                    }
                    break;
                }
            }

            spawnedGameObjects[4] = SpawnParticlesAtPosition(worldPos);

            return spawnedGameObjects;
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
        GameObject cacheParent = new GameObject("Cached Particle Systems");

        //Populate missing attributes
        foreach (GibBase gib in gibs) {

            //Update the gib's manager.
            gib.GibManager = this;

            //Check to see if any elements are unavailable
            if (gib.particleSystemMaterial == null) {
                gib.particleSystemMaterial = defaultGib.particleSystemMaterial;
            }

            if (gib.particleSystemPreset == null) {
                gib.particleSystemPreset = defaultGib.particleSystemPreset;
            }

            gib.CacheParticleSystem(cacheParent.transform);
        }

        //Get list of gib indexes and sizes.
        gibSizes = new List<KeyValuePair<int, float>>(gibInstanceGameObjects.Length);
        for (int i = 0; i < gibInstanceGameObjects.Length; i++) {
            gibSizes.Add(new KeyValuePair<int, float>(
                i,
                gibInstanceGameObjects[i].GetComponent<MeshFilter>().sharedMesh.bounds.size.magnitude * gibInstanceGameObjects[i].transform.localScale.magnitude));
        }
        //Sort by size.
        gibSizes.Sort(delegate (KeyValuePair<int, float> pair1, KeyValuePair<int, float> pair2) {
            return pair1.Value.CompareTo(pair2.Value);
        });
    }
}