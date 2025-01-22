using TMPro;
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
    public Button smallTowerButton;
    public Button mediumTowerButton;
    public Button largeTowerButton;
    public Button wallsButton;
    public Button gatehouseButton;

    [Header("UI Elements")]
    public TextMeshProUGUI costText;

    private void Start()
    {
        if (buildingPlacementController == null)
        {
            throw new System.Exception($"BuildingUIController on object {transform} doesn't have a BuildingPlacementController assigned.");
        }

        townHallButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.TownHall));
        barracksButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Barracks));
        warehouseButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Warehouse));
        marketButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Market));
        farmButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Farm));
        mineButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Mine));
        woodcutterButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Woodcutter));
        smallTowerButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.SmallTower));
        mediumTowerButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.MediumTower));
        largeTowerButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.LargeTower));
        wallsButton.onClick.AddListener(() => OnWallsButtonClicked());
        gatehouseButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Gatehouse));

        AddHoverListeners(townHallButton, BuildingType.TownHall);
        AddHoverListeners(barracksButton, BuildingType.Barracks);
        AddHoverListeners(warehouseButton, BuildingType.Warehouse);
        AddHoverListeners(marketButton, BuildingType.Market);
        AddHoverListeners(farmButton, BuildingType.Farm);
        AddHoverListeners(mineButton, BuildingType.Mine);
        AddHoverListeners(woodcutterButton, BuildingType.Woodcutter);
        AddHoverListeners(smallTowerButton, BuildingType.SmallTower);
        AddHoverListeners(mediumTowerButton, BuildingType.MediumTower);
        AddHoverListeners(largeTowerButton, BuildingType.LargeTower);
        AddHoverListeners(wallsButton, BuildingType.Walls);
        AddHoverListeners(gatehouseButton, BuildingType.Gatehouse);
    }

    private void OnBuildingButtonClicked(BuildingType buildingType)
    {
        if(buildingType == BuildingType.TownHall && buildingPlacementController.HasTownhallBeenBuilt)
        {
            return;
        }

        if(!buildingPlacementController.HasTownhallBeenBuilt && buildingType != BuildingType.TownHall)
        {
            return;
        }

        buildingPlacementController.StartBuildingPlacement(buildingType);
    }

    private void OnWallsButtonClicked()
    {
        if (!buildingPlacementController.HasTownhall)
        {
            return;
        }

        buildingPlacementController.StartLookingForWallPlacementLocation();
    }

    private void AddHoverListeners(Button button, BuildingType buildingType)
    {
        var eventTrigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        var pointerEnter = new UnityEngine.EventSystems.EventTrigger.Entry
        {
            eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter
        };
        pointerEnter.callback.AddListener((_) => ShowBuildingCost(buildingType));
        eventTrigger.triggers.Add(pointerEnter);

        var pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry
        {
            eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit
        };
        pointerExit.callback.AddListener((_) => HideBuildingCost());
        eventTrigger.triggers.Add(pointerExit);
    }

    private void ShowBuildingCost(BuildingType buildingType)
    {
        if (buildingPlacementController.TryGetBuildingPrefab(buildingType, out GameObject buildingPrefab))
        {
            var buildingBase = buildingPrefab.GetComponent<BuildingBase>();

            if (buildingBase != null)
            {
                costText.text = GetCostText(buildingBase.Costs);
                costText.gameObject.SetActive(true);
            }
        }
    }

    private void HideBuildingCost()
    {
        costText.gameObject.SetActive(false);
    }

    private string GetCostText(Cost[] costs)
    {
        var costText = "";

        foreach (var cost in costs)
        {
            if (!string.IsNullOrWhiteSpace(costText))
            {
                costText += ", ";
            }

            costText += $"{cost.resourceType}: {cost.amount}";
        }

        return costText.TrimEnd();
    }
}
