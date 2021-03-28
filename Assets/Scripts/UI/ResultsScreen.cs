using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
    public TextMeshProUGUI EndScoreTMP;
    public TextMeshProUGUI EndTimeTMP;
    public TextMeshProUGUI ScoreTMP;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI FinalGradeTMP;

    private int FinalScore;
    private string FinalGrade;
    

    // Start is called before the first frame update
    void Start()
    {
        EndScoreTMP.text = ScoreTMP.text;
        EndTimeTMP.text = timerText.text;
        
        int.TryParse(ScoreTMP.text, out FinalScore);

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

        FinalGradeTMP.text = "" + FinalGrade;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
