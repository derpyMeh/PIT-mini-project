using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button restartButton;
    [SerializeField] Button quitButton;
    bool menuActive = false;

    // Update is called once per frame
    void Update()
    {
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
