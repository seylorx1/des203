using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;


public class PauseMenu : MonoBehaviour {
    public GameObject PauseMenuUI;

    public GameObject LeftSettings;
    public GameObject LeftMap;
    public GameObject LeftLevelInfo;

    public GameObject RightSettings;
    public GameObject RightMap;
    public GameObject RightLevelInfo;

    public GameObject SettingsScreen;

    public GameObject MapScreen;

    public GameObject LevelInfoScreen;

    public TextMeshProUGUI SelectedScreen;

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

    public void ClickSettings()
    {
        LevelInfoScreen.SetActive(false);
        MapScreen.SetActive(false);
        SettingsScreen.SetActive(true);

        LeftMap.SetActive(false);
        LeftSettings.SetActive(false);
        LeftLevelInfo.SetActive(true);

        RightSettings.SetActive(false);
        RightLevelInfo.SetActive(false);
        RightMap.SetActive(true);

        SelectedScreen.text = "Settings";
    }

    public void ClickMap()
    {
        LevelInfoScreen.SetActive(false);
        SettingsScreen.SetActive(false);
        MapScreen.SetActive(true);

        LeftMap.SetActive(false);
        LeftLevelInfo.SetActive(false);
        LeftSettings.SetActive(true);

        RightSettings.SetActive(false);
        RightMap.SetActive(false);
        RightLevelInfo.SetActive(true);

        SelectedScreen.text = "Map";
    }

    public void ClickLevelInfo()
    {
        SettingsScreen.SetActive(false);
        MapScreen.SetActive(false);
        LevelInfoScreen.SetActive(true);

        LeftLevelInfo.SetActive(false);
        LeftSettings.SetActive(false);
        LeftMap.SetActive(true);

        RightMap.SetActive(false);
        RightLevelInfo.SetActive(false);
        RightSettings.SetActive(true);

        SelectedScreen.text = "Level Info";
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
