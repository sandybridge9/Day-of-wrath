using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PathfindingGridUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField nodeRadiusInput; // Updated to TMP_InputField
    public Button regenerateGridButton;

    [Header("Pathfinding Grid Reference")]
    public PathfindingGrid pathfindingGrid;

    private void Start()
    {
        // Assign button and input field listeners
        regenerateGridButton.onClick.AddListener(OnRegenerateGridClicked);
        nodeRadiusInput.onEndEdit.AddListener(OnNodeRadiusChanged);

        // Initialize the input field with the current nodeRadius
        nodeRadiusInput.text = pathfindingGrid.nodeRadius.ToString("F2"); // Display with 2 decimal places
    }

    private void OnNodeRadiusChanged(string value)
    {
        if (float.TryParse(value, out float newRadius) && newRadius > 0)
        {
            pathfindingGrid.SetNodeRadius(newRadius);
            Debug.Log($"Updated nodeRadius to {newRadius}");
        }
        else
        {
            Debug.LogWarning("Invalid nodeRadius value entered.");
            nodeRadiusInput.text = pathfindingGrid.nodeRadius.ToString("F2"); // Revert to the current value
        }
    }

    private void OnRegenerateGridClicked()
    {
        pathfindingGrid.GenerateGrid();
        Debug.Log("Grid regenerated manually via UI.");
    }
}
