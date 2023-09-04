using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class SelectionController : MonoBehaviour
{
    [HideInInspector]
    public LayerMask SelectableLayers;

    [HideInInspector]
    public LayerMask GroundLayers;

    [HideInInspector]
    public List<UnitBase> SelectedUnits = new List<UnitBase>();

    [HideInInspector]
    public BuildingBase SelectedBuilding = null;

    [HideInInspector]
    public bool IsSelectionActive
    {
        get
        {
            return SelectedUnits.Count > 0
                || SelectedBuilding != null;
        }
    }

    [HideInInspector]
    public bool AnySelectedUnits
    {
        get { return SelectedUnits.Count > 0; }
    }

    [HideInInspector]
    public bool IsBuildingSelected
    {
        get { return SelectedBuilding != null; }
    }

    public RectTransform SelectionBox;
    public bool IsSelectionBoxActive = false;

    private Vector3 selectionBoxStartingPosition;
    private Vector3 selectionBoxFinishingPosition;
    private Vector2[] selectionBoxCorners;

    private RaycastHit hit;

    private MeshCollider selectionBox;
    private Mesh selectionMesh;
    private Vector3[] vertices;
    private Vector3[] vecs;

    private void Start()
    {
        SelectableLayers = LayerMask.GetMask(
            GlobalSettings.Layers.UnitLayer,
            GlobalSettings.Layers.BuildingLayer);

        GroundLayers = LayerMask.GetMask(GlobalSettings.Layers.TerrainLayer);
    }

    public void PointSelect()
    {
        var mousePosition = Input.mousePosition;
        var castPointRay = Camera.main.ScreenPointToRay(mousePosition);

        if (!Physics.Raycast(castPointRay, out var raycastHit, Mathf.Infinity, SelectableLayers))
        {
            ClearSelection();

            return;
        }

        TrySelectHitObject(raycastHit);
    }
    public void StartBoxSelection(Vector2 selectionBoxStartingPosition)
    {
        ClearSelection();

        SelectionBox.gameObject.SetActive(true);
        IsSelectionBoxActive = true;

        this.selectionBoxStartingPosition = selectionBoxStartingPosition;
    }

    public void ContinueBoxSelection()
    {
        var currentMousePosition = Input.mousePosition;
        var width = currentMousePosition.x - selectionBoxStartingPosition.x;
        var height = currentMousePosition.y - selectionBoxStartingPosition.y;
        SelectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        SelectionBox.anchoredPosition = selectionBoxStartingPosition.Vector2() + new Vector2(width / 2, height / 2);
    }

    public void FinishBoxSelection()
    {
        vertices = new Vector3[4];
        vecs = new Vector3[4];
        int i = 0;
        selectionBoxFinishingPosition = Input.mousePosition;
        selectionBoxCorners = GetBoundingBox(selectionBoxStartingPosition, selectionBoxFinishingPosition);

        foreach (Vector2 corner in selectionBoxCorners)
        {
            Ray ray = Camera.main.ScreenPointToRay(corner);

            if (Physics.Raycast(ray, out hit, 50000.0f, GroundLayers))
            {
                vertices[i] = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                vecs[i] = ray.origin - hit.point;
                Debug.DrawLine(Camera.main.ScreenToWorldPoint(corner), hit.point, Color.red, 1.0f);
            }
            i++;
        }

        selectionMesh = GenerateSelectionMesh(vertices, vecs);

        selectionBox = gameObject.AddComponent<MeshCollider>();
        selectionBox.sharedMesh = selectionMesh;
        selectionBox.convex = true;
        selectionBox.isTrigger = true;

        Debug.Log(selectionBox.transform.position);

        //if (!Input.GetKey(KeyCode.LeftShift))
        //{
        //    selected_table.deselectAll();
        //}

        Destroy(selectionBox, 0.02f);

        SelectionBox.sizeDelta = new Vector2(0, 0);
        SelectionBox.anchoredPosition = new Vector2(0, 0);
        SelectionBox.gameObject.SetActive(false);
        IsSelectionBoxActive = false;
    }

    public void ClearSelection()
    {
        if (AnySelectedUnits)
        {
            SelectedUnits
                .ForEach(selectedUnit => selectedUnit.IsSelected = false);
            SelectedUnits
                .Clear();
        }

        if (IsBuildingSelected)
        {
            SelectedBuilding.IsSelected = false;
            SelectedBuilding = null;
        }
    }



    private Vector2[] GetBoundingBox(Vector2 point1, Vector2 point2)
    {
        var bottomLeft = Vector3.Min(point1, point2);
        var topRight = Vector3.Max(point1, point2);

        Vector2[] corners =
        {
            new Vector2(bottomLeft.x, topRight.y),
            new Vector2(topRight.x, topRight.y),
            new Vector2(bottomLeft.x, bottomLeft.y),
            new Vector2(topRight.x, bottomLeft.y)
        };

        return corners;
    }

    private Mesh GenerateSelectionMesh(Vector3[] corners, Vector3[] vecs)
    {
        Vector3[] verts = new Vector3[8];
        int[] tris = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 };

        for (int i = 0; i < 4; i++)
        {
            verts[i] = corners[i];
        }

        for (int j = 4; j < 8; j++)
        {
            verts[j] = corners[j - 4] + vecs[j - 4];
        }

        Mesh selectionMesh = new Mesh();
        selectionMesh.vertices = verts;
        selectionMesh.triangles = tris;

        return selectionMesh;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("something entered the atmosphere");
        var selectableObject = collision.gameObject.GetComponent<SelectableObject>();

        if (selectableObject != null)
        {
            AddSelectedObjectToList(selectableObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("something entered the atmosphere");
        var selectableObject = other.gameObject.GetComponent<SelectableObject>();
        
        if(selectableObject != null)
        {
            AddSelectedObjectToList(selectableObject);
        }
    }

    private void TrySelectHitObject(RaycastHit raycastHit)
    {
        var hitGameObject = raycastHit.collider.gameObject;

        if (hitGameObject == null)
        {
            return;
        }

        var selectableObject = hitGameObject.GetComponent<SelectableObject>();

        if (selectableObject == null)
        {
            ClearSelection();

            return;
        }

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            ClearSelection();
        }

        AddSelectedObjectToList(selectableObject);
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
}
