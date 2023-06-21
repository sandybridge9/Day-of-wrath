using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public LayerMask SelectableLayers;

    [HideInInspector]
    public List<UnitBase> SelectedUnits = new List<UnitBase>();

    [HideInInspector]
    public bool IsSelectionActive
    {
        get
        {
            return SelectedUnits.Count > 0;
        }
    }

    [HideInInspector]
    public bool AnySelectedUnits
    {
        get
        {
            return SelectedUnits.Count > 0;
        }
    }

    void Update()
    {
        Debug.Log(SelectedUnits.Count);
        Select();
    }

    private void Select()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClearSelection();

            var mousePosition = Input.mousePosition;
            var castPointRay = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(castPointRay, out var raycastHit, Mathf.Infinity, SelectableLayers))
            {
                TrySelectHitObject(raycastHit);
            }
        }
    }

    private void TrySelectHitObject(RaycastHit raycastHit)
    {
        var hitGameObject = raycastHit.collider.gameObject;

        if (hitGameObject != null)
        {
            var selectableObject = hitGameObject.GetComponent<SelectableObject>();

            if (selectableObject != null)
            {
                AddSelectedObjectToList(selectableObject);
            }
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
                Debug.Log("Selected a Building");
                break;

            default:
                break;
        }
    }

    private void ClearSelection()
    {
        if (IsSelectionActive)
        {
            SelectedUnits.ForEach(
                selectedUnit => selectedUnit.IsSelected = false);

            SelectedUnits.Clear();

            Debug.Log("Deselected Units and Buildings");
        }
    }
}
