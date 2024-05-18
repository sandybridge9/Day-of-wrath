using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static bool IsGamePaused = false;

    public GameObject PauseMenuUI;
    public GameObject PauseMenuUIBackground;

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
        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
