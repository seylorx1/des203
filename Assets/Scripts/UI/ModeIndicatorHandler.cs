using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeIndicatorHandler : MonoBehaviour
{
    public Image move, snip;
    bool showMove = false, showSnip = false;

    public PlayerController playerController;

    bool lastSnip = false; 

    // Update is called once per frame
    void Update()
    {
        Color moveColor = move.color;
        moveColor.a = Mathf.Lerp(moveColor.a, showMove ? 1.0f : 0.0f, Time.unscaledDeltaTime * 3.0f);
        

        Color snipColor = snip.color;
        snipColor.a = Mathf.Lerp(snipColor.a, showSnip ? 1.0f : 0.0f, Time.unscaledDeltaTime * 3.0f);
        

        if(playerController.Snip != lastSnip) {
            if (playerController.Snip) {
                StartCoroutine(ShowSnipIndicator());
                StopCoroutine(ShowMoveIndicator());

                snipColor.a = 1.0f;
                moveColor.a = 0.0f;
                showMove = false;
            }
            else {
                StartCoroutine(ShowMoveIndicator());
                StopCoroutine(ShowSnipIndicator());

                moveColor.a = 1.0f;
                snipColor.a = 0.0f;
                showSnip = false;
            }
            lastSnip = playerController.Snip;
        }

        move.color = moveColor;
        snip.color = snipColor;
    }

    public IEnumerator ShowMoveIndicator() {
        showMove = true;
        yield return new WaitForSecondsRealtime(1.0f);
        showMove = false;
        yield return null;
    }

    public IEnumerator ShowSnipIndicator() {
        showSnip = true;
        yield return new WaitForSecondsRealtime(1.0f);
        showSnip = false;
        yield return null;
    }
}
