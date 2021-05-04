using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    public GameObject loadingScreen;

    public void Awake() {
        //loadingScreen.SetActive(false);
    }

    public void PlayGame()
    {
        loadingScreen.SetActive(true);
        StartCoroutine(LoadMainScene());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator LoadMainScene() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Ben's Test Space");

        while(!asyncLoad.isDone) {
            yield return null;
        }

        SceneManager.UnloadSceneAsync("Main Menu");
    }
}
