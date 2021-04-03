using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class PlayerWorldInteraction : MonoBehaviour {
   
    public ScoreNotifier scoreNotifier;
    public ObjectiveHandler objectiveHandler;

    public GameObject chuteDoor;
    public Light RClight;
    public Light Buttonlight;
    public GameObject Results;

    public GameObject ObjectiveNotifier;

    public GameObject OpenGateTick;
    public TextMeshProUGUI OpenGateInfoText;
    public TextMeshProUGUI OpenGateNotifierText;
    public GameObject EscapeTankTick;

    public GameObject OpenChuteTick;
    public TextMeshProUGUI OpenChuteInfoText;
    public TextMeshProUGUI OpenChuteNotifierText;

    public int NumberOfPearls;
    private int CollectedPearls = 0;
    public TextMeshProUGUI PearlsText;

    public float gravityScale = 5;
    public float MaxVelocity = 5;
    public float Acceleration = 5;

    private PlayerController playerController;
    private Rigidbody crabRigidbody;
    private Animation ObjectNotifierAnim;



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
        ObjectNotifierAnim = ObjectiveNotifier.GetComponent<Animation>();
        PearlsText.text = CollectedPearls + " / " + NumberOfPearls;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Collectable") {
            Score += 50;
            CollectedPearls = CollectedPearls + 1;
            PearlsText.text = CollectedPearls + " / " + NumberOfPearls;
            Destroy(other.gameObject);
        }

        if (other.tag == "GateObjective")
        {

            OpenGateInfoText.text = "Open Castle Gate";
            OpenGateNotifierText.text = OpenGateInfoText.text;

            ObjectNotifierAnim.Play("PopUp");
            Destroy(other.gameObject);
        }

        if (other.tag == "TankObjective")
        {
            ObjectNotifierAnim.Play("PopUpEscapeTank");
            EscapeTankTick.SetActive(true);
            Destroy(other.gameObject);
        }

        if (other.tag == "ChuteObjective")
        {
            OpenChuteNotifierText.text = "Open Rubbish Chute";
            OpenChuteInfoText.text = OpenChuteNotifierText.text;

            ObjectNotifierAnim.Play("PopUp");
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

            OpenChuteNotifierText.text = "Open Rubbish Chute";
            OpenChuteInfoText.text = OpenChuteNotifierText.text;

            ObjectNotifierAnim.Play("PopUpOpenChute");
            OpenChuteTick.SetActive(true);


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

        if (other.tag == "Fish Tank")
        {
            objectiveHandler.escapeFishTank = true;
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
