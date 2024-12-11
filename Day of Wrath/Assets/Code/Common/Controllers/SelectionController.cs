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
    public bool IsAnythingSelected => AnySelectedUnits || IsBuildingSelected;

    [HideInInspector]
    public bool IsMultiSelectEnabled { get; set; }

    public void PointSelect()
    {
        var mousePosition = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(mousePosition);

        if (!Physics.Raycast(ray, out var raycastHit, Mathf.Infinity, LayerManager.SelectableLayers))
        {
            ClearSelection();
            return;
        }

        HandleRaycastHit(raycastHit);
    }

    private void HandleRaycastHit(RaycastHit raycastHit)
    {
        var selectableObject = raycastHit.collider.GetComponent<SelectableObject>();

        if (selectableObject != null)
        {
            HandleSelectableObject(selectableObject);

            return;
        }

        selectableObject = raycastHit.collider.GetComponentInParent<SelectableObject>();

        if (selectableObject != null)
        {
            HandleSelectableObject(selectableObject);

            return;
        }

        selectableObject = raycastHit.collider.GetComponentInChildren<SelectableObject>();

        if (selectableObject != null)
        {
            HandleSelectableObject(selectableObject);

            return;
        }
    }

    private void HandleSelectableObject(SelectableObject selectableObject)
    {
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
