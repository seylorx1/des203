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

    public int NumberOfCrabs;
    private int CollectedCrabs = 0;
    public TextMeshProUGUI CrabsText;

    public TextMeshProUGUI CPauseNotifier;
    public TextMeshProUGUI RPauseNotifier;
    public TextMeshProUGUI APauseNotifier;
    public TextMeshProUGUI BPauseNotifier;

    public TextMeshProUGUI CUINotifier;
    public TextMeshProUGUI RUINotifier;
    public TextMeshProUGUI AUINotifier;
    public TextMeshProUGUI BUINotifier;

    public float gravityScale = 5;
    public float MaxVelocity = 5;
    public float Acceleration = 5;

    private PlayerController playerController;
    private Rigidbody crabRigidbody;
    private Animation ObjectNotifierAnim;

    private bool ChuteOpen = false;
    private bool TankEscaped = false;




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
        CrabsText.text = CollectedCrabs + " / " + NumberOfCrabs;
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
            Score += 100;
            Time.timeScale = 0.0f;
            Results.SetActive(true);
        }

        if (other.tag == "C")
        {
            CPauseNotifier.color = new Color32(255, 196, 52, 255);
            CUINotifier.color = new Color32(255, 196, 52, 255);
            ObjectNotifierAnim.Play("CRABPopup");
            Score += 100;
            Destroy(other.gameObject);
        }

        if (other.tag == "R")
        {
            RPauseNotifier.color = new Color32(255, 196, 52, 255);
            RUINotifier.color = new Color32(255, 196, 52, 255);
            ObjectNotifierAnim.Play("CRABPopup");
            Score += 100;
            Destroy(other.gameObject);
        }

        if (other.tag == "A")
        {
            APauseNotifier.color = new Color32(255, 196, 52, 255);
            AUINotifier.color = new Color32(255, 196, 52, 255);
            ObjectNotifierAnim.Play("CRABPopup");
            Score += 100;
            Destroy(other.gameObject);
        }

        if (other.tag == "B")
        {
            BPauseNotifier.color = new Color32(255, 196, 52, 255);
            BUINotifier.color = new Color32(255, 196, 52, 255);
            ObjectNotifierAnim.Play("CRABPopup");
            Score += 100;
            Destroy(other.gameObject);
        }

        if (other.tag == "SaveCrab")
        {
            CollectedCrabs = CollectedCrabs + 1;
            CrabsText.text = CollectedCrabs + " / " + NumberOfCrabs;
            Score += 50;
            Destroy(other.gameObject);
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
                Destroy(other.gameObject);
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
