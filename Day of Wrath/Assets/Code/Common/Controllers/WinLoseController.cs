using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class WinLoseController : MonoBehaviour
{
    [Header("Controllers")]
    public BuildingPlacementController playerBuildingController;

    public bool EnemyTownhallHasBeenBuilt = false;
    public bool EnemyHasTownhall = false;

    [Header("UI Elements")]
    public GameObject winLosePanel;
    public TMP_Text winLoseText;
    public string mainMenuSceneName = "MainMenu";

    private bool gameEnded = false;

    private void Update()
    {
        if (gameEnded)
        {
            return;
        }

        //if (!EnemyTownhallHasBeenBuilt)
        //{
        //    LookForEnemyTownhall();
        //}

        //CheckWinCondition();
        //CheckLoseCondition();
    }

    private void LookForEnemyTownhall()
    {
        Debug.Log("Searching for enemy townhall");
        var townhalls = FindObjectsOfType<TownhallBuilding>();

        if(townhalls != null && townhalls.Any(x => x.Team == Team.Enemy))
        {
            Debug.Log("Found enemy townhall");
            var enemyTownhall = townhalls.First(x => x.Team == Team.Enemy);

            enemyTownhall.OnBuildingPlaced();

            EnemyTownhallHasBeenBuilt = true;
            EnemyHasTownhall = true;
        }
    }

    private void CheckWinCondition()
    {
        // Win when the enemy's townhall is destroyed, but only if it has been built
        if (EnemyTownhallHasBeenBuilt && !EnemyHasTownhall)
        {
            TriggerWin();
        }
    }

    private void CheckLoseCondition()
    {
        // Lose when the player's townhall is destroyed, but only if it has been built
        if (playerBuildingController.HasTownhallBeenBuilt && !playerBuildingController.HasTownhall)
        {
            TriggerLoss();
        }
    }

    private void TriggerWin()
    {
        gameEnded = true;
        ShowWinLosePanel("You Won!");
    }

    private void TriggerLoss()
    {
        gameEnded = true;
        ShowWinLosePanel("You Lost!");
    }

    private void ShowWinLosePanel(string message)
    {
        winLosePanel.SetActive(true);
        winLoseText.text = message;
        Debug.Log(message);
    }

    public void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
