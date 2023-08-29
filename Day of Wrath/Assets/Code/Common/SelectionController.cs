using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    [HideInInspector]
    public LayerMask SelectableLayers;

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
    private Vector2 selectionBoxStartingPosition;

    private void Start()
    {
        SelectableLayers = LayerMask.GetMask(
            nameof(SelectableObjectType.Unit),
            nameof(SelectableObjectType.Building));
    }

    public void Select()
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
        SelectionBox.anchoredPosition = selectionBoxStartingPosition + new Vector2(width / 2, height / 2);
    }

    public void FinishBoxSelection()
    {
        SelectionBox.gameObject.SetActive(false);
        IsSelectionBoxActive = false;

        var min = SelectionBox.anchoredPosition - (SelectionBox.sizeDelta / 2);
        var max = SelectionBox.anchoredPosition + (SelectionBox.sizeDelta / 2);

        //foreach (Unit unit in player.units)
        //{
        //    Vector3 screenPos = cam.WorldToScreenPoint(unit.transform.position);

        //    if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
        //    {
        //        selectionUnits.Add(unit);
        //        unit.ToggleSelectionVisual(true);
        //    }
        //}
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

    public void ClearSelection()
    {
        if (AnySelectedUnits)
        {
            SelectedUnits
                .ForEach(selectedUnit => selectedUnit.IsSelected = false);
            SelectedUnits
                .Clear();
        }

        if(IsBuildingSelected)
        {
            SelectedBuilding.IsSelected = false;
            SelectedBuilding = null;
        }
    }
}
