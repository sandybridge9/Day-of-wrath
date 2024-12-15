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

    public RectTransform SelectionBox;

    public bool IsBoxSelecting = false;
    private Vector2 boxStartPosition;

    private LayerMask unitLayerMask;

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
        // Get the corners of the selection box in screen space
        Rect selectionRect = GetSelectionRect();

        // Clear the current selection
        ClearSelection();

        // Find all units in the scene
        UnitBase[] allUnits = FindObjectsOfType<UnitBase>();

        foreach (UnitBase unit in allUnits)
        {
            Vector3 unitScreenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (selectionRect.Contains(unitScreenPos, true))
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

    //private void SelectUnitsInBox()
    //{
    //    Vector3[] corners = GetScreenSpaceCorners();

    //    // Raycast to get the world-space points on the ground plane
    //    Vector3 bottomLeft = ScreenToGroundPoint(corners[0]);
    //    Vector3 topRight = ScreenToGroundPoint(corners[2]);

    //    // Calculate the center and size for the box collider
    //    Vector3 center = (bottomLeft + topRight) / 2f;
    //    Vector3 size = new Vector3(Mathf.Abs(topRight.x - bottomLeft.x), 10f, Mathf.Abs(topRight.z - bottomLeft.z));

    //    // Perform OverlapBox to detect units within the selection box
    //    Collider[] hits = Physics.OverlapBox(center, size / 2f, Quaternion.identity, unitLayerMask);

    //    ClearSelection();

    //    foreach (var hit in hits)
    //    {
    //        UnitBase unit = hit.GetComponent<UnitBase>();
    //        if (unit != null)
    //        {
    //            unit.IsSelected = true;
    //            SelectedUnits.Add(unit);
    //            Debug.Log($"Unit selected: {unit.name}");
    //        }
    //    }
    //}

    //private Vector3 ScreenToGroundPoint(Vector3 screenPoint)
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(screenPoint);
    //    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerManager.GroundLayers))
    //    {
    //        return hit.point;
    //    }

    //    // Fallback to return a default point if raycast fails
    //    return Vector3.zero;
    //}

    //private Vector3[] GetScreenSpaceCorners()
    //{
    //    Vector3[] corners = new Vector3[4];
    //    SelectionBox.GetWorldCorners(corners);

    //    // Convert world-space corners to screen-space points
    //    for (int i = 0; i < corners.Length; i++)
    //    {
    //        corners[i] = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[i]);
    //    }

    //    return corners;
    //}


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
