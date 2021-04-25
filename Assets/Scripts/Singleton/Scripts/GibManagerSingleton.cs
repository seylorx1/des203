using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GibManagerSingleton : SingletonScriptableObject {

    public GameObject[] gibInstanceGameObjects;
    private List<KeyValuePair<int, float>> gibSizes;

    [System.Serializable]
    public class GibBase {

        public Color gibColor;
        public Material particleSystemMaterial;
        public GameObject particleSystemPrefab;

        public GibManagerSingleton GibManager { private get; set; }

        public GameObject SpawnParticlesAtPosition(Vector3 worldPos) {
            //Create new gameobject and assign a new particle system
            GameObject gibParticleInstance = Instantiate(particleSystemPrefab, worldPos, Quaternion.identity);
            gibParticleInstance.name = "Gib Particle Instance";

            //Get Particle System
            ParticleSystem ps = gibParticleInstance.GetComponent<ParticleSystem>();
            ParticleSystemRenderer ps_r = gibParticleInstance.GetComponent<ParticleSystemRenderer>();

            //Apply main module properties
            ParticleSystem.MainModule ps_mm = ps.main;
            ps_mm.startColor = gibColor;

            //Apply material
            ps_r.material = particleSystemMaterial;

            return gibParticleInstance;
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

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Singleton/GibManagerSingleton")]
    public static void CreateAsset() {
        ObjectHelper.CreateAsset<GibManagerSingleton>();
    }
#endif

    public override void OnAwake() {
        //Populate missing attributes
        foreach (GibCustom gib in gibs) {

            //Update the gib's manager.
            gib.GibManager = this;

            //Check to see if any elements are unavailable
            if (gib.particleSystemMaterial == null) {
                gib.particleSystemMaterial = defaultGib.particleSystemMaterial;
            }

            if (gib.particleSystemPrefab == null) {
                gib.particleSystemPrefab = defaultGib.particleSystemPrefab;
            }
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