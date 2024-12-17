using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public bool IsGamePaused = false;

    public GameObject PauseMenuUI;
    public GameObject PauseMenuUIBackground;
    public GameObject SettingsMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(IsGamePaused)
            {
                ResumeGame();

                return;
            }

            PauseGame();
        }
    }

    public void ResumeGame()
    {
        PauseMenuUI.SetActive(false);
        PauseMenuUIBackground.SetActive(false);
        SettingsMenuUI.SetActive(false);

        Time.timeScale = 1f;

        IsGamePaused = false;
    }

    private void PauseGame()
    {
        PauseMenuUI.SetActive(true);
        PauseMenuUIBackground.SetActive(true);

        Time.timeScale = 0f;

        IsGamePaused = true;
    }

    public void LoadMainMenu()
    {
        IsGamePaused = false;

        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
