using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerWorldInteraction : MonoBehaviour {
    public ScoreNotifier scoreNotifier;

    private PlayerController playerController;

    private int _score = 0;
    public int Score {
        get {
            return _score;
        }
        private set {
            scoreNotifier.AddScore(value - _score, value);

            _score = value;
        }
    }

    void Start() {
        playerController = GetComponent<PlayerController>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Collectable") {
            Score += 20;

            //Destroy(other.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.collider.tag == "Hazard") {
            playerController.TakeHeat(5.0f * Time.deltaTime);
        }
    }
}
