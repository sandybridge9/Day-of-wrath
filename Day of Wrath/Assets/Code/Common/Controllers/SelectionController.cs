using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    [HideInInspector]
    public List<UnitBase> SelectedUnits = new List<UnitBase>();

    [HideInInspector]
    public BuildingBase SelectedBuilding = null;

    [HideInInspector]
    public bool AnySelectedUnits()
    {
        SelectedUnits.RemoveAll(x => x == null);

        return SelectedUnits.Count > 0;
    }

    [HideInInspector]
    public bool IsBuildingSelected => SelectedBuilding != null;

    [HideInInspector]
    public bool IsAnythingSelected => AnySelectedUnits() || IsBuildingSelected;

    [HideInInspector]
    public bool IsMultiSelectEnabled { get; set; }

    public RectTransform SelectionBox;

    public bool IsBoxSelecting = false;
    private Vector2 boxStartPosition;

    private LayerMask unitLayerMask;

    public delegate void BuildingSelectedHandler(BuildingBase building);
    public event BuildingSelectedHandler OnBuildingSelected;

    private void Start()
    {
        if (SelectionBox != null)
        {
            SelectionBox.gameObject.SetActive(false);
        }

        unitLayerMask = LayerManager.UnitLayer;
    }

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

        if (building != null
            && !building.IsFromDifferentTeam(Team.Friendly))
        {
            building.IsSelected = true;
            SelectedBuilding = building;

            OnBuildingSelected?.Invoke(building);

            Debug.Log("Building selected: " + building.name);
        }
    }

    private void SelectUnit(UnitBase unit)
    {
        if (!IsMultiSelectEnabled)
        {
            ClearSelection();
        }

        if (unit != null
            && !unit.IsFromDifferentTeam(Team.Friendly))
        {
            if (!SelectedUnits.Contains(unit))
            {
                unit.IsSelected = true;
                SelectedUnits.Add(unit);

                Debug.Log("Unit selected: " + unit.name);
            }
        }
    }

    public void StartBoxSelection(Vector2 startPosition)
    {
        IsBoxSelecting = true;
        boxStartPosition = startPosition;
        SelectionBox.gameObject.SetActive(true);

        UpdateBoxSelection(startPosition);
    }

    public void UpdateBoxSelection(Vector2 currentMousePosition)
    {
        var width = currentMousePosition.x - boxStartPosition.x;
        var height = currentMousePosition.y - boxStartPosition.y;

        SelectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        SelectionBox.anchoredPosition = boxStartPosition + new Vector2(width / 2, height / 2);
    }

    public void FinishBoxSelection()
    {
        IsBoxSelecting = false;
        SelectionBox.gameObject.SetActive(false);

        SelectUnitsInBox();
    }

    private void SelectUnitsInBox()
    {
        Rect selectionRect = GetSelectionRect();

        ClearSelection();

        UnitBase[] allUnits = FindObjectsOfType<UnitBase>();

        foreach (UnitBase unit in allUnits)
        {
            Vector3 unitScreenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (selectionRect.Contains(unitScreenPos, true)
                && !unit.IsFromDifferentTeam(Team.Friendly))
            {
                unit.IsSelected = true;
                SelectedUnits.Add(unit);
                Debug.Log($"Unit selected: {unit.name}");
            }
        }
    }

    private Rect GetSelectionRect()
    {
        Vector2 start = boxStartPosition;
        Vector2 end = Input.mousePosition;

        float xMin = Mathf.Min(start.x, end.x);
        float yMin = Mathf.Min(start.y, end.y);
        float width = Mathf.Abs(end.x - start.x);
        float height = Mathf.Abs(end.y - start.y);

        return new Rect(xMin, yMin, width, height);
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
        OnBuildingSelected?.Invoke(null);
    }
}
