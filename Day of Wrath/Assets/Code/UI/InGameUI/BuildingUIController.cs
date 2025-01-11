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

    [Header("UI Elements")]
    public TextMeshProUGUI buildingCostText;

    private void Start()
    {
        if (buildingPlacementController == null)
        {
            throw new System.Exception($"BuildingUIController on object {transform} doesn't have a BuildingPlacementController assigned.");
        }

        // Assign button click listeners
        townHallButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.TownHall));
        barracksButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Barracks));
        warehouseButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Warehouse));
        marketButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Market));
        farmButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Farm));
        mineButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Mine));
        woodcutterButton.onClick.AddListener(() => OnBuildingButtonClicked(BuildingType.Woodcutter));

        // Assign hover listeners
        AddHoverListeners(townHallButton, BuildingType.TownHall);
        AddHoverListeners(barracksButton, BuildingType.Barracks);
        AddHoverListeners(warehouseButton, BuildingType.Warehouse);
        AddHoverListeners(marketButton, BuildingType.Market);
        AddHoverListeners(farmButton, BuildingType.Farm);
        AddHoverListeners(mineButton, BuildingType.Mine);
        AddHoverListeners(woodcutterButton, BuildingType.Woodcutter);
    }

    private void OnBuildingButtonClicked(BuildingType buildingType)
    {
        buildingPlacementController.StartBuildingPlacement(buildingType);
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
                buildingCostText.text = GetCostText(buildingBase.Costs);
                buildingCostText.gameObject.SetActive(true);
            }
        }
    }

    private void HideBuildingCost()
    {
        buildingCostText.gameObject.SetActive(false);
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
