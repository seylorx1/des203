using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioZoneDebug : MonoBehaviour {
    private AudioZone audioZone;
    private AudioSource audioSource;
    public float maxVolume = 0.5f;
    // Start is called before the first frame update
    void Start() {
        audioZone = GetComponent<AudioZone>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        audioSource.volume = audioZone.audioGradientMono * maxVolume;

        Vector2 panVec = new Vector2(audioZone.audioGradientLeft, audioZone.audioGradientRight).normalized;
        audioSource.panStereo = panVec.y - panVec.x;
    }
}
