using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    //public RectTransform SelectionBox;

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

    [HideInInspector]
    public bool IsSelectionBoxActive = false;

    //private Vector2 selectionBoxUIStartingPosition;

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

    // Box Selection Logic (currently commented out)
    /*
    public void StartBoxSelection(Vector2 selectionBoxStartingPosition)
    {
        ClearSelection();

        SelectionBox.gameObject.SetActive(true);
        SelectionBox.sizeDelta = new Vector2(0, 0);
        SelectionBox.anchoredPosition = new Vector2(0, 0);

        IsSelectionBoxActive = true;
        selectionBoxUIStartingPosition = selectionBoxStartingPosition;
    }

    public void ContinueBoxSelection()
    {
        var currentMousePosition = Input.mousePosition;
        var width = currentMousePosition.x - selectionBoxUIStartingPosition.x;
        var height = currentMousePosition.y - selectionBoxUIStartingPosition.y;
        SelectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        SelectionBox.anchoredPosition = selectionBoxUIStartingPosition + new Vector2(width / 2, height / 2);
    }

    public void FinishBoxSelection()
    {
        var selectionMesh = GenerateSelectionMesh();

        var selectionBox = gameObject.AddComponent<MeshCollider>();
        selectionBox.sharedMesh = selectionMesh;
        selectionBox.convex = true;
        selectionBox.isTrigger = true;

        Destroy(selectionBox, 0.02f);

        SelectionBox.sizeDelta = new Vector2(0, 0);
        SelectionBox.anchoredPosition = new Vector2(0, 0);
        SelectionBox.gameObject.SetActive(false);
        IsSelectionBoxActive = false;
    }

    private Mesh GenerateSelectionMesh()
    {
        var selectionBoxCorners = GetSelectionBoxCorners(selectionBoxUIStartingPosition, Input.mousePosition);
        var worldPointVertices = new List<Vector3>();
        var edgeVectors = new List<Vector3>();

        foreach (var selectionBoxCorner in selectionBoxCorners)
        {
            var ray = Camera.main.ScreenPointToRay(selectionBoxCorner);

            if (Physics.Raycast(ray, out var rayCastHit, 50000.0f, GlobalSettings.Layers.GroundLayers))
            {
                worldPointVertices.Add(new Vector3(rayCastHit.point.x, rayCastHit.point.y, rayCastHit.point.z));
                edgeVectors.Add(ray.origin - rayCastHit.point);
            }
        }

        if (worldPointVertices.Count < 4 || edgeVectors.Count < 4)
        {
            return null;
        }

        var selectionMeshVertices = new Vector3[8];
        int[] selectionMeshTriangles = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 };

        for (int i = 0; i < 4; i++)
        {
            selectionMeshVertices[i] = worldPointVertices[i];
        }

        for (int j = 4; j < 8; j++)
        {
            selectionMeshVertices[j] = worldPointVertices[j - 4] + edgeVectors[j - 4];
        }

        var selectionMesh = new Mesh();
        selectionMesh.vertices = selectionMeshVertices;
        selectionMesh.triangles = selectionMeshTriangles;

        return selectionMesh;
    }

    private List<Vector2> GetSelectionBoxCorners(Vector2 selectionBoxStartingPosition, Vector2 selectionBoxFinishingPosition)
    {
        var bottomLeftPoint = Vector3.Min(selectionBoxStartingPosition, selectionBoxFinishingPosition);
        var topRightPoint = Vector3.Max(selectionBoxStartingPosition, selectionBoxFinishingPosition);

        var corners = new List<Vector2>
        {
            new Vector2(bottomLeftPoint.x, topRightPoint.y),
            new Vector2(topRightPoint.x, topRightPoint.y),
            new Vector2(bottomLeftPoint.x, bottomLeftPoint.y),
            new Vector2(topRightPoint.x, bottomLeftPoint.y)
        };

        return corners;
    }

    private void OnTriggerEnter(Collider other)
    {
        var selectableObject = other.gameObject.GetComponent<SelectableObject>();

        if (selectableObject != null)
        {
            AddSelectedObjectToList(selectableObject);
        }
    }

    private void AddSelectedObjectToList(SelectableObject selectableObject)
    {
        switch (selectableObject.Type)
        {
            case SelectableObjectType.Unit:
                var unitBase = selectableObject.GetComponent<UnitBase>();

                if (unitBase != null)
                {
                    unitBase.IsSelected = true;
                    SelectedUnits.Add(unitBase);

                    Debug.Log("Selected a Unit");
                }

                break;

            case SelectableObjectType.Building:
                var buildingBase = selectableObject.GetComponent<BuildingBase>();

                if (buildingBase != null)
                {
                    buildingBase.IsSelected = true;
                    SelectedBuilding = buildingBase;

                    Debug.Log("Selected a Building");
                }

                break;

            default:
                break;
        }
    }
    */
}
