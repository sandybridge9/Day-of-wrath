using System;
using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    public GameObject SelectionIndicatorPrefab;

    public float GroundOffset = 0.05f;
    public float ScaleMultiplier = 1.5f;

    private GameObject selectionIndicatorInstance;

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

        yPosition += GroundOffset;
        return new Vector3(transform.position.x, yPosition, transform.position.z);
    }

    private Vector3 GetIndicatorScale()
    {
        var collider = GetRelevantCollider();

        if (collider != null)
        {
            return GetColliderScaleVector(collider);
        }

        throw new Exception($"Couldn't find a collider on this GameObject {transform}");
    }

    private Collider GetRelevantCollider()
    {
        var placementTrigger = transform.Find("PlacementTrigger");

        if (placementTrigger != null && placementTrigger.TryGetComponent<Collider>(out var placementTriggerCollider))
        {
            return placementTriggerCollider;
        }

        if (TryGetComponent<Collider>(out var mainCollider))
        {
            return mainCollider;
        }

        return null;
    }

    private Vector3 GetColliderScaleVector(Collider collider)
    {
        var maxSize = Mathf.Max(collider.bounds.size.x, collider.bounds.size.z);
        var scale = maxSize * ScaleMultiplier;

        return new Vector3(scale, scale, scale);
    }
}
