using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class AudioZone : MonoBehaviour {

    public float
        audioGradientMono,
        audioGradientLeft,
        audioGradientRight;

    public Vector3 paddingSize;

    public AnimationCurve falloffCurve;

    [Header("Note: bounds are always hidden in builds.")]
    [SerializeField] private bool showBoundsInGame = false;

    private bool smoothTransitionAudio = false;
    private Bounds
        mrBounds,
        mrBoundsPadding;

    private Transform
        leftEar = null,
        rightEar = null,
        padding = null;


    private void Awake() {
#if !UNITY_EDITOR
        showBoundsInGame = false;
#endif
        leftEar = GameObject.FindGameObjectWithTag("Left Ear").transform;
        rightEar = GameObject.FindGameObjectWithTag("Right Ear").transform;

        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) {
            switch (mr.tag) {
                case "Audio Zone":
                    mrBounds = mr.bounds;
                    break;
                case "Audio Zone Padding":
                    mrBoundsPadding = mr.bounds;
                    padding = mr.transform;
                    break;
                default:
                    break;
            }

            if (Application.isPlaying) {
                mr.enabled = showBoundsInGame;
            }
        }
    }

    private void Update() {
#if UNITY_EDITOR
        {
            paddingSize = Vector3.Max(Vector3.zero, paddingSize);
            smoothTransitionAudio = paddingSize.sqrMagnitude > 0.0f;

            if (padding != null) {
                padding.gameObject.SetActive(smoothTransitionAudio);
                padding.localPosition = Vector3.zero;
                padding.localRotation = Quaternion.identity;

                padding.localScale =
                    new Vector3(
                    (2.0f * (paddingSize.x + transform.lossyScale.x * 0.5f) / transform.lossyScale.x),
                    (2.0f * (paddingSize.y + transform.lossyScale.y * 0.5f) / transform.lossyScale.y),
                    (2.0f * (paddingSize.z + transform.lossyScale.z * 0.5f) / transform.lossyScale.z));
            }
        }
#endif

        if (Application.isPlaying) {
            audioGradientLeft = GetGradientAtPosition(leftEar.position);
            audioGradientRight = GetGradientAtPosition(rightEar.position);
            audioGradientMono = 0.5f * (audioGradientLeft + audioGradientRight);
        }
#if UNITY_EDITOR
        else {
            audioGradientMono = 0.0f;
            audioGradientLeft = 0.0f;
            audioGradientRight = 0.0f;
        }
#endif
    }

    public float GetGradientAtPosition(Vector3 worldPos) {
        if (mrBounds.Contains(worldPos)) {
            return 1.0f;
        }
        else if (smoothTransitionAudio && mrBoundsPadding.Contains(worldPos)) {

            Vector3 relDir = (worldPos - mrBounds.center);
            relDir.x = Mathf.Abs(relDir.x);
            relDir.y = Mathf.Abs(relDir.y);
            relDir.z = Mathf.Abs(relDir.z);

            Vector3 dirVol = Vector3.one - Vector3.Scale(relDir - mrBounds.extents, paddingSize.Invert());

            return falloffCurve.Evaluate(Mathf.Clamp01(Mathf.Min(dirVol.x, dirVol.y, dirVol.z)));

        }
        return 0.0f;
    }
}
