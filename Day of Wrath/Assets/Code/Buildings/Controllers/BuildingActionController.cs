using UnityEngine;

public class BuildingActionController : MonoBehaviour
{
    private BarrackActionController barrackActionController;
    private SelectionController selectionController;

    private void Start()
    {
        barrackActionController = GetComponent<BarrackActionController>();
        selectionController = GetComponent<SelectionController>();
    }

    public void TryTrainUnit()
    {
        if (selectionController.SelectedBuilding is BarrackBuilding barrack)
        {
            barrackActionController.SetSelectedBarrack(barrack);
            barrackActionController.TrainUnit();
        }
    }

    public void DeleteSelectedBuilding()
    {
        if (selectionController.SelectedBuilding != null)
        {
            var building = selectionController.SelectedBuilding;

            building.OnBuildingDestroyed();
            selectionController.ClearSelection();

            Destroy(building.gameObject);
        }
    }
}
