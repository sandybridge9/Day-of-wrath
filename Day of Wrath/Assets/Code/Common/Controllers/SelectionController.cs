using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    [HideInInspector]
    public List<UnitBase> SelectedUnits = new List<UnitBase>();

    [HideInInspector]
    public BuildingBase SelectedBuilding = null;

    [HideInInspector]
    public bool AnySelectedUnits => SelectedUnits.Count > 0;

    [HideInInspector]
    public bool IsBuildingSelected => SelectedBuilding != null;

    [HideInInspector]
    public bool IsMultiSelectEnabled { get; set; }

    public void PointSelect()
    {
        var mousePosition = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(mousePosition);

        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, LayerManager.SelectableLayers))
        {
            ClearSelection();
            return;
        }

        var selectableObject = hit.collider.GetComponent<SelectableObject>();

        if (selectableObject == null)
        {
            return;
        }

        if (selectableObject.Type == SelectableObjectType.Building)
        {
            SelectBuilding(selectableObject as BuildingBase);
        }
        else if (selectableObject.Type == SelectableObjectType.Unit)
        {
            SelectUnit(selectableObject as UnitBase);
        }
    }

    private void SelectBuilding(BuildingBase building)
    {
        ClearSelection();

        if (building != null)
        {
            building.IsSelected = true;
            SelectedBuilding = building;
            Debug.Log("Building selected: " + building.name);
        }
    }

    private void SelectUnit(UnitBase unit)
    {
        if (!IsMultiSelectEnabled)
        {
            ClearSelection();
        }

        if (unit != null)
        {
            unit.IsSelected = true;
            if (!SelectedUnits.Contains(unit))
            {
                SelectedUnits.Add(unit);
                Debug.Log("Unit selected: " + unit.name);
            }
        }
    }

    public void ClearSelection()
    {
        if (IsBuildingSelected)
        {
            SelectedBuilding.IsSelected = false;
            SelectedBuilding = null;
        }

        foreach (var unit in SelectedUnits)
        {
            unit.IsSelected = false;
        }

        SelectedUnits.Clear();
    }
}
