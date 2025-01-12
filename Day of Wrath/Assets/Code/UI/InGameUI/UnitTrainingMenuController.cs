using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitTrainingMenuController : MonoBehaviour
{
    [Header("Unit Buttons")]
    public Button warriorButton;
    public Button archerButton;

    [Header("UI Elements")]
    public TextMeshProUGUI costText;

    private BuildingActionController buildingActionController;
    private BarrackActionController barrackActionController;

    private void Start()
    {
        buildingActionController = FindObjectOfType<BuildingActionController>();
        barrackActionController = FindObjectOfType<BarrackActionController>();

        var selectionController = FindObjectOfType<SelectionController>();
        selectionController.OnBuildingSelected += HandleBuildingSelection;

        warriorButton.onClick.AddListener(() => TrainUnit("Warrior"));
        // archerButton.onClick.AddListener(() => TrainUnit("Archer"));

        AddHoverListeners(warriorButton, "Warrior");
        // AddHoverListeners(archerButton, "Archer");

        gameObject.SetActive(false);
    }

    private void AddHoverListeners(Button button, string unitType)
    {
        var eventTrigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        var pointerEnter = new UnityEngine.EventSystems.EventTrigger.Entry
        {
            eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter
        };
        pointerEnter.callback.AddListener((_) => ShowUnitCost(unitType));
        eventTrigger.triggers.Add(pointerEnter);

        var pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry
        {
            eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit
        };
        pointerExit.callback.AddListener((_) => HideCostText());
        eventTrigger.triggers.Add(pointerExit);
    }

    private void ShowUnitCost(string unitType)
    {
        if (barrackActionController != null)
        {
            var unitPrefab = unitType switch
            {
                "Warrior" => barrackActionController.WarriorPrefab,
                // "Archer" => barrackActionController.ArcherPrefab,
                _ => null
            };

            if (unitPrefab != null)
            {
                var unitBase = unitPrefab.GetComponent<UnitBase>();
                if (unitBase != null)
                {
                    costText.text = GetCostText(unitBase.Costs);
                    costText.gameObject.SetActive(true);
                }
            }
        }
    }

    private void HideCostText()
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

    private void HandleBuildingSelection(BuildingBase building)
    {
        if (building is BarrackBuilding)
        {
            ShowMenu();
        }
        else
        {
            HideMenu();
        }
    }

    private void ShowMenu()
    {
        gameObject.SetActive(true);
    }

    private void HideMenu()
    {
        gameObject.SetActive(false);
    }

    private void TrainUnit(string unitType)
    {
        if (buildingActionController != null)
        {
            switch (unitType)
            {
                case "Warrior":
                    buildingActionController.TryTrainUnit();
                    break;
            }
        }
    }
}