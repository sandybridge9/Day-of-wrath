using UnityEngine;
using UnityEngine.UI;

public class BuildingUIController : MonoBehaviour
{
    [Header("Building Placement Controller")]
    public BuildingPlacementController buildingPlacementController;

    [Header("Building Buttons")]
    public Button townHallButton;
    public Button barracksButton;
    public Button warehouseButton;
    public Button marketButton;
    public Button farmButton;
    public Button mineButton;
    public Button woodcutterButton;

    private void Start()
    {
        // Assign button click listeners
        townHallButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.TownHall));
        barracksButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Barracks));
        warehouseButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Warehouse));
        marketButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Market));
        farmButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Farm));
        mineButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Mine));
        woodcutterButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Woodcutter));

        if (buildingPlacementController == null)
        {
            throw new System.Exception($"BuildingUIController on object {transform} doesn't have a BuildingPlacementController assigned.");
        }
    }

    private void OnBuildingButtonClicked(BuildingType buildingType)
    {
        buildingPlacementController.StartBuildingPlacement(buildingType);
    }
}
