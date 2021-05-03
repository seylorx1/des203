using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
    public TextMeshProUGUI EndScoreTMP;
    public TextMeshProUGUI EndTimeTMP;
    public TextMeshProUGUI EndDeathsTMP;
    public TextMeshProUGUI ScoreTMP;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI FinalScoreTMP;
    public TextMeshProUGUI FinalGradeTMP;
    public TextMeshProUGUI BonusesNamesTMP;
    public TextMeshProUGUI BonusesValuesTMP;

    public GameObject TimerUI;
    public GameObject CrabController;
    public GameObject SaveCrabPrefab;

    private int FinalScore;
    private string FinalGrade;
    private float FinalTime;
    private int FinalCrabs;
    private int FinalPearls;
    private int FinalDeaths;

    private Timer TimerScript;
    private PlayerWorldInteraction WorldScript;
    private PlayerController playerController;
    private Savecrab SaveCrabScript;
    

    // Start is called before the first frame update
    void Start()
    {
        int.TryParse(ScoreTMP.text, out FinalScore);

        TimerScript = TimerUI.GetComponent<Timer>();
        WorldScript = CrabController.GetComponent<PlayerWorldInteraction>();
        playerController = CrabController.GetComponent<PlayerController>();
        SaveCrabScript = SaveCrabPrefab.GetComponent<Savecrab>();

        FinalDeaths = playerController.Deaths;
        FinalCrabs = SaveCrabScript.CollectedCrabs;
        FinalPearls = WorldScript.CollectedPearls;
        FinalTime = TimerScript.seconds;

        EndDeathsTMP.text = "" + FinalDeaths;
        EndScoreTMP.text = ScoreTMP.text;
        EndTimeTMP.text = timerText.text;

        BonusesNamesTMP.text = "" + "Escaped";
        BonusesValuesTMP.text = "" + "100";
        FinalScore = FinalScore + 100;

        if (FinalDeaths == 0)
        {
            BonusesNamesTMP.text = BonusesNamesTMP.text + "" + "\n" + "Untouchable";
            BonusesValuesTMP.text = BonusesValuesTMP.text + "" + "\n" + "100";
            FinalScore = FinalScore + 100;
        }

        if (FinalTime <= 300)
        {
            BonusesNamesTMP.text = BonusesNamesTMP.text + "" + "\n" + "Speed Run";
            BonusesValuesTMP.text = BonusesValuesTMP.text + "" + "\n" + "400";
            FinalScore = FinalScore + 400;
        }
        else if (FinalTime <= 480)
        {
            BonusesNamesTMP.text = BonusesNamesTMP.text + "" + "\n" + "Extra Quick Crab";
            BonusesValuesTMP.text = BonusesValuesTMP.text + "" + "\n" + "300";
            FinalScore = FinalScore + 300;
        }
        else if (FinalTime <= 600)
        {
            BonusesNamesTMP.text = BonusesNamesTMP.text + "" + "\n" + "Quick Crab";
            BonusesValuesTMP.text = BonusesValuesTMP.text + "" + "\n" + "200";
            FinalScore = FinalScore + 200;
        }

        if (FinalCrabs >= 1)
        {
            BonusesNamesTMP.text = BonusesNamesTMP.text + "" + "\n" + "All Crabs Saved";
            BonusesValuesTMP.text = BonusesValuesTMP.text + "" + "\n" + "100";
            FinalScore = FinalScore + 100;
        }

        if (FinalPearls >= 12)
        {
            BonusesNamesTMP.text = BonusesNamesTMP.text + "" + "\n" + "All Pearls Collected";
            BonusesValuesTMP.text = BonusesValuesTMP.text + "" + "\n" + "200";
            FinalScore = FinalScore + 200;
        }

        if (FinalScore <= 400)
        {
            FinalGrade = "D";
        }
        else if (FinalScore >= 1100)
        {
            FinalGrade = "S";
        }
        else if (FinalScore >= 800)
        {
            FinalGrade = "A";
        }
        else if (FinalScore >= 600)
        {
            FinalGrade = "B";
        }
        else if (FinalScore >= 400)
        {
            FinalGrade = "C";
        }

        FinalScoreTMP.text = "" + FinalScore;
        FinalGradeTMP.text = "" + FinalGrade;
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Continue()
    {
        SceneManager.LoadScene("MainScene");
    }

}
