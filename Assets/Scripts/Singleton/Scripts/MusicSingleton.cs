using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MusicSingleton : SingletonScriptableObject
{
    public float maximumVolume = 0.5f;

    [System.Serializable]
    public class MusicTrack {
        public AudioClip trackClip;

        [Header("Higher priority tracks will be played over lower priority tracks.")]
        public int priority = 0;
    }

    [SerializeField] private List<MusicTrack> musicTracks;



    private List<MusicSource> allMusicSources = new List<MusicSource>();

    private const uint MAX_CURRENT_MUSIC_SOURCES = 4;
    private MusicSource[] currentMusicSources;

    public void Register(MusicSource musicSource) {
        if (musicSource.trackClip == null) {
            Debug.LogError("MusicSource has no audio clip!", musicSource);
        }
        else {
            if(musicTracks.Count > 0) {
                foreach (MusicTrack musicTrack in musicTracks) {
                    if (musicTrack.trackClip.Equals(musicSource.trackClip)) {
                        musicSource.musicTrack = musicTrack;
                        musicSource.audioSource.clip = musicTrack.trackClip;
                        musicSource.audioSource.Play();
                        allMusicSources.Add(musicSource);
                        return;
                    }
                }
                Debug.LogError($"Track clip ({musicSource.trackClip.name}) not in MusicSingleton!", musicSource);
            }
            else {
                Debug.LogError("MusicSingleton has no registed music tracks!", this);
            }
        }
    }

    public bool Unregister(MusicSource musicSource) {
        return allMusicSources.Remove(musicSource);
    }

    public override void OnUpdate() {
        currentMusicSources = new MusicSource[MAX_CURRENT_MUSIC_SOURCES];
        allMusicSources.ForEach(source => {
            if(source.audioZone.audioGradientMono > 0) {

                MusicSource moveDown = null;
                for(int i = 0; i < MAX_CURRENT_MUSIC_SOURCES; i++) {

                    if(currentMusicSources[i] == null) {
                        //Fills array.
                        if (moveDown != null) {
                            currentMusicSources[i] = moveDown;
                            moveDown = null;
                        }
                        else {
                            currentMusicSources[i] = source;
                        }
                        break;
                    }

                    if(moveDown != null && moveDown.musicTrack.priority >= currentMusicSources[i].musicTrack.priority) {
                        MusicSource foo = currentMusicSources[i];
                        currentMusicSources[i] = moveDown;
                        moveDown = foo; 
                        continue;
                    }

                    if (source.musicTrack.priority > currentMusicSources[i].musicTrack.priority ||
                            (source.musicTrack.priority == currentMusicSources[i].musicTrack.priority &&
                            source.audioZone.audioGradientMono >= currentMusicSources[i].audioZone.audioGradientMono)) {
                        //If source is greater than or equal-and-louder.
                        moveDown = currentMusicSources[i];
                        currentMusicSources[i] = source;
                        continue;
                    }
                }

                //Discard. If move down did exist, it was too low-priority to be considered!
                moveDown = null;

                //currentMusicSources is now sorted by priority first and loudness second.

                //one music source is guaranteed to exist.
                currentMusicSources[0].audioSource.volume = currentMusicSources[0].audioZone.audioGradientMono * maximumVolume;
                float previousPriorityVolume = 0.0f;

                for (int i = 1; i < MAX_CURRENT_MUSIC_SOURCES; i++) {
                    if(currentMusicSources[i] != null) {
                        if(currentMusicSources[i-1].musicTrack.priority != currentMusicSources[i].musicTrack.priority) {

                            //indexOfFirstWithPriority = i;
                            previousPriorityVolume = currentMusicSources[i - 1].audioSource.volume;
                        }

                        currentMusicSources[i].audioSource.volume =
                                currentMusicSources[i].audioZone.audioGradientMono * (maximumVolume - previousPriorityVolume);
                    }
                }

            }
            else {
                source.audioSource.volume = 0.0f;
            }
        });
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Singleton/MusicSingleton")]
    public static void CreateAsset() {
        ObjectHelper.CreateAsset<MusicSingleton>();
    }
#endif
}
