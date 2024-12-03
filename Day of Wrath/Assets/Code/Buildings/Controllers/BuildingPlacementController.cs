using UnityEngine;

public class BuildingPlacementController : MonoBehaviour
{
    public GameObject buildingPrefab;

    private LayerMask groundLayers;
    private LayerMask blockingLayers;

    private GameObject currentBuilding;
    private Renderer buildingRenderer;

    public Color canBuildColor = new(0, 1, 0, 0.5f);
    public Color cannotBuildColor = new(1, 0, 0, 0.5f);

    private Color originalColor;

    public float rotationSpeed = GlobalSettings.Buildings.RotationSpeed;

    private bool isPlacingBuilding = false;
    private bool canPlaceBuilding = false;

    private void Start()
    {
        groundLayers = LayerManager.GroundLayers;
        blockingLayers = LayerManager.BlockingLayers;
    }

    private void Update()
    {
        if (isPlacingBuilding)
        {
            UpdateBuildingPosition();
            CheckPlacementValidity();
        }
    }

    public void StartBuildingPlacement()
    {
        if (currentBuilding != null)
        {
            CancelPlacement();
        }

        currentBuilding = Instantiate(buildingPrefab);
        buildingRenderer = currentBuilding.GetComponent<Renderer>();
        originalColor = buildingRenderer.material.color;

        SetBuildingTransparency(canBuildColor);
        isPlacingBuilding = true;
    }

    public void RotateBuilding(int direction)
    {
        if (currentBuilding != null)
        {
            currentBuilding.transform.Rotate(Vector3.up, direction * rotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateBuildingPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayers))
        {
            float buildingHalfHeight = currentBuilding.GetComponent<Collider>().bounds.extents.y;
            currentBuilding.transform.position = hit.point + new Vector3(0, buildingHalfHeight, 0);
        }
    }

    private void CheckPlacementValidity()
    {
        Vector3 buildPosition = currentBuilding.transform.position;
        Vector3 halfExtents = currentBuilding.GetComponent<Collider>().bounds.extents * 0.9f;

        Collider buildingCollider = currentBuilding.GetComponent<Collider>();
        buildingCollider.enabled = false;

        Collider[] colliders = Physics.OverlapBox(buildPosition, halfExtents, Quaternion.identity, blockingLayers);
        buildingCollider.enabled = true;

        canPlaceBuilding = colliders.Length == 0;
        SetBuildingTransparency(canPlaceBuilding ? canBuildColor : cannotBuildColor);
    }

    public void PlaceBuilding()
    {
        if (!canPlaceBuilding)
        {
            Debug.Log("Cannot place the building here. Position is invalid (red).");
            return;
        }

        Debug.Log("Building placed successfully!");
        SetBuildingTransparency(originalColor);
        isPlacingBuilding = false;
        currentBuilding = null;
    }

    public void CancelPlacement()
    {
        if (currentBuilding != null)
        {
            Destroy(currentBuilding);
        }
        isPlacingBuilding = false;
        currentBuilding = null;
    }

    public bool IsPlacingBuilding()
    {
        return isPlacingBuilding;
    }

    private void SetBuildingTransparency(Color color)
    {
        if (buildingRenderer != null)
        {
            buildingRenderer.material.color = color;
        }
    }
}
