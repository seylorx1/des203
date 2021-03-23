using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour {
    private bool GameIsPaused = false;
    public GameObject PauseMenuUI;
    // Start is called before the first frame update
    void Start() {
        registerInputEvents();
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
    private void registerInputEvents() {
        InputManagerData data = (InputManagerData)InputManager.Instance.SingletonBaseRef.Data;

        data.pause.performed += CheckPaused;
        data.pause.canceled += CheckPaused;
    }
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
