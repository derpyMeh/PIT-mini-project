using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] CombatSystem combatSystem;
    [SerializeField] GameObject gameOverMenu;
    [Header("Buttons")]
    [SerializeField] Button restartButton;
    [SerializeField] Button quitButton;
    bool menuActive = false;

    // Update is called once per frame
    void Update()
    {
        if (!combatSystem.PlayerAlive && !menuActive)
        {
            // uipanelGameObject.SetActive(true);
            menuActive = true;
            Time.timeScale = 0f;
        }

        if (menuActive)
        {
            // listen for button pressed
            // if restart is pressed:
            Time.timeScale = 1f;
            menuActive = false;

        }

        restartButton.onClick.AddListener(RestartListener);
        quitButton.onClick.AddListener(QuitListener);
    }

    void RestartListener()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    void QuitListener()
    {
        Application.Quit();
    }
}
