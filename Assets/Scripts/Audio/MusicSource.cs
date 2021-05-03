using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioZone))]
[RequireComponent(typeof(AudioSource))]
public class MusicSource : MonoBehaviour{
    [HideInInspector] public AudioZone audioZone;
    [HideInInspector] public AudioSource audioSource;

    [Header("This track must also be registered on the music singleton!")]
    public AudioClip trackClip;

    //This is set by the music singleton.
    [HideInInspector] public MusicSingleton.MusicTrack musicTrack;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        audioZone = GetComponent<AudioZone>();
    }

    private void Start() {
        SingletonManager.Instance.GetSingleton<MusicSingleton>().Register(this);
    }
    private void OnDisable() {
        SingletonManager.Instance.GetSingleton<MusicSingleton>().Unregister(this);
    }
}
