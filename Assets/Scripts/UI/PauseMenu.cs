using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
    
{
    private bool GameIsPaused = false;
    public GameObject PauseMenuUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void CheckPaused(InputAction.CallbackContext ctx)
    {
        ctx.ReadValueAsButton();
        if (GameIsPaused)
        {
            Resume();
        }
        else

        {
            Pause();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Return()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    private void registerInputEvents()
    {
        InputManagerData data = (InputManagerData)InputManager.Instance.SingletonBaseRef.Data;

        data.pause.performed += CheckPaused;
        data.pause.canceled += CheckPaused;
    }
}
