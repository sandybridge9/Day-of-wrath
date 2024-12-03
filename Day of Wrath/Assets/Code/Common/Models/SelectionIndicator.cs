using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    public GameObject SelectionIndicatorPrefab; // Assign the prefab in the Inspector

    private GameObject selectionIndicatorInstance;

    private float groundOffset = 0.05f;

    public void Show()
    {
        if (SelectionIndicatorPrefab == null)
        {
            Debug.LogWarning("SelectionIndicator: SelectionCirclePrefab is not assigned.");
            return;
        }

        if (selectionIndicatorInstance == null)
        {
            selectionIndicatorInstance = Instantiate(SelectionIndicatorPrefab, GetIndicatorPosition(), Quaternion.Euler(90, 0, 0));
            selectionIndicatorInstance.transform.localScale = GetIndicatorScale();
            selectionIndicatorInstance.transform.SetParent(transform, true);
        }
    }

    public void Hide()
    {
        if (selectionIndicatorInstance != null)
        {
            Destroy(selectionIndicatorInstance);
        }
    }


    private Vector3 GetIndicatorPosition()
    {
        Collider collider = GetComponent<Collider>();
        float yPosition = transform.position.y;

        if (collider != null)
        {
            yPosition = collider.bounds.min.y;
        }

        yPosition += groundOffset;
        return new Vector3(transform.position.x, yPosition, transform.position.z);
    }

    private Vector3 GetIndicatorScale()
    {
        // Scale the circle based on the object's collider size
        float scale = 1.5f; // Default scale multiplier
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            scale = Mathf.Max(collider.bounds.size.x, collider.bounds.size.z) * 2f;
        }
        return new Vector3(scale, scale, scale);
    }
}
