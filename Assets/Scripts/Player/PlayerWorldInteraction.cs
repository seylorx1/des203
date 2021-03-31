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
    public GameObject Results;

    public float gravityScale = 5;
    public float MaxVelocity = 5;
    public float Acceleration = 5;

    private PlayerController playerController;
    private Rigidbody crabRigidbody;

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
        crabRigidbody = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Collectable") {
            Score += 50;

            Destroy(other.gameObject);
        }

        if (other.tag == "Exit")
        {
            Time.timeScale = 0.0f;
            Results.SetActive(true);
        }

        if (other.tag == "Button")
        {
            chuteDoor.SetActive(true);
            RClight.color = Color.green;
            Buttonlight.color = Color.green;
        }

        if (other.tag == "Float")
        {
            crabRigidbody.useGravity = false;
            crabRigidbody.AddForce(transform.up * -gravityScale);
            crabRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            //Stolen Lyes' movement code for in the Float Zone
            if (Mathf.Abs(playerController.inputLS.x) > 0.1f)
            { 
                Vector3 targetVelocity = transform.right * -playerController.inputLS.x * Acceleration;

                Vector3 velocityChange = (targetVelocity - crabRigidbody.velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -MaxVelocity, MaxVelocity);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -MaxVelocity, MaxVelocity);

                velocityChange.y = 0.15f; //Adding a little bit of lift stops the crab from clipping the ground and tipping *as much*.

                crabRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Float")
        {
            crabRigidbody.useGravity = true;
            crabRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
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
