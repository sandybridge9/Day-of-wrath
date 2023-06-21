using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public LayerMask SelectableLayers;

    [HideInInspector]
    public List<SelectableObject> SelectedObjects = new List<SelectableObject>();

    [HideInInspector]
    public bool IsSelectionActive
    {
        get
        {
            return SelectedObjects.Count > 0;
        }
    }

    void Update()
    {
        Debug.Log(SelectedObjects.Count);
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
                var hitGameObject = raycastHit.collider.gameObject;

                if (hitGameObject != null)
                {
                    var selectableObject = hitGameObject.GetComponent<SelectableObject>();

                    if (selectableObject != null)
                    {
                        selectableObject.IsSelected = true;
                        SelectedObjects.Add(selectableObject);

                        Debug.Log("Selected a SelectableObject");
                    }
                }

                return;
            }
        }
    }

    private void ClearSelection()
    {
        if (IsSelectionActive)
        {
            SelectedObjects.ForEach(
                selectedObject => selectedObject.IsSelected = false);

            SelectedObjects.Clear();

            Debug.Log("Deselected");
        }
    }
}
