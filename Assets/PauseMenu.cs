using Future;
using ScriptableObjectArchitecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    [SerializeField] GameObject mainMenu;
    [SerializeField] GameEvent PauseGame;
    [SerializeField] GameEvent ContinueGame;
    [SerializeField] GameEvent SetResetButton;
    [SerializeField] GameEvent UnsetResetButton;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused || mainMenu.activeSelf)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }

    public void Resume ()
    {
        pauseMenuUI.SetActive(false);
        ContinueGame.Raise();
        SetResetButton.Raise();
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    void Pause()
    {
        PauseGame.Raise();
        UnsetResetButton.Raise();
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
}
