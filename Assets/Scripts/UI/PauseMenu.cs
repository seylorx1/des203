using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour {
    public GameObject PauseMenuUI;

    private bool GameIsPaused = false;
    // Start is called before the first frame update
    void Start() {
        InputSingleton inputSingleton = SingletonManager.Instance.GetSingleton<InputSingleton>();
        inputSingleton.pause.performed += CheckPaused;
        inputSingleton.pause.canceled += CheckPaused;
    }

    public void Resume() {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    public void Pause() {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene("Main Menu");
    }

    #region Inputs
    public void CheckPaused(InputAction.CallbackContext ctx) {

        if (ctx.ReadValueAsButton()) {
            if (GameIsPaused) {
                Resume();
            }
            else {
                Pause();
            }
        }
    }
    #endregion
}
