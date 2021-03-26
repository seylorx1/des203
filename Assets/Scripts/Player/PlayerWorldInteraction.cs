using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class PlayerWorldInteraction : MonoBehaviour {
    public ScoreNotifier scoreNotifier;

    public GameObject chuteDoor;
    public Light RClight;
    public Light Buttonlight;

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
            Score += 50;

            Destroy(other.gameObject);
        }

        if (other.tag == "Exit")
        {
            SceneManager.LoadScene("Main Menu");
        }

        if (other.tag == "Button")
        {
            chuteDoor.SetActive(true);
            RClight.color = Color.green;
            Buttonlight.color = Color.green;
        }


    }

    private void OnCollisionStay(Collision collision) {
        if (collision.collider.tag == "Hazard") {
            playerController.TakeHeat(5.0f * Time.deltaTime);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Hazard")
        {
            playerController.TakeHeat(5.0f * Time.deltaTime);

        }
    }
}
