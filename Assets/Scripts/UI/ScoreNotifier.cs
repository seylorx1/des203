using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreNotifier : MonoBehaviour {
    public TextMeshProUGUI ScoreTMP;

    private int currentScore = 0;
    private int visualScore = 0;


    private Animation scoreNotifyAnim;
    private TextMeshProUGUI scoreNotifyText;
    private List<int> stashedScores = new List<int>();
    // Start is called before the first frame update
    void Start() {
        scoreNotifyAnim = GetComponent<Animation>();
        scoreNotifyText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update() {
        if (!scoreNotifyAnim.isPlaying) {

            UpdateScoreText();

            if (stashedScores != null && stashedScores.Count > 0) {
                scoreNotifyText.text = "+" + stashedScores[0];

                scoreNotifyAnim.Play();
                visualScore += stashedScores[0];

                stashedScores.RemoveAt(0);
            }
            else {
                scoreNotifyText.text = "";

                visualScore = currentScore; //Keeps visual score updated and stops desync.
            }
        }
    }

    public void AddScore(int amount, int currentScore) {
        stashedScores.Add(amount);
        this.currentScore = currentScore;
    }

    public float GetAnimationLength() {
        return scoreNotifyAnim.clip.length;
    }

    //NOT THE NOTIFIER
    private void UpdateScoreText() {
        ScoreTMP.text = "" + visualScore;
    }
}
