using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementController : MonoBehaviour
{
    public GameObject buildingPrefab;

    private LayerMask groundLayers;
    private LayerMask blockingLayers;

    public float rotationSpeed = GlobalSettings.Buildings.RotationSpeed;

    private bool isPlacingBuilding = false;

    private bool couldPlaceBuildingLastFrame = false;
    private bool canPlaceBuilding = false;

    public Color canBuildColor = new(0, 1, 0, 0.5f);
    public Color cannotBuildColor = new(1, 0, 0, 0.5f);

    private GameObject currentBuilding;

    private List<Material> originalMaterials = new List<Material>();
    private List<Material> currentMaterials = new List<Material>();
    private List<Renderer> renderers = new List<Renderer>();

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

    public bool IsPlacingBuilding()
    {
        return isPlacingBuilding;
    }

    public void StartBuildingPlacement()
    {
        if (currentBuilding != null)
        {
            CancelPlacement();
        }

        currentBuilding = Instantiate(buildingPrefab);
        renderers.AddRange(currentBuilding.GetComponents<Renderer>());
        renderers.AddRange(currentBuilding.GetComponentsInChildren<Renderer>());

        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                originalMaterials.Add(new Material(material));
                currentMaterials.Add(material);
            }
        }

        isPlacingBuilding = true;
        CheckPlacementValidity();
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

    public void CancelPlacement()
    {
        originalMaterials.Clear();
        currentMaterials.Clear();
        renderers.Clear();

        if (currentBuilding != null)
        {
            Destroy(currentBuilding);
        }

        isPlacingBuilding = false;
        currentBuilding = null;
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

        if (couldPlaceBuildingLastFrame == canPlaceBuilding)
        {
            SetBuildingTransparency(canPlaceBuilding ? canBuildColor : cannotBuildColor);
        }

        couldPlaceBuildingLastFrame = canPlaceBuilding;
    }

    public void PlaceBuilding()
    {
        if (!canPlaceBuilding)
        {
            Debug.Log("Cannot place the building here. Position is invalid (red).");
            return;
        }

        Debug.Log("Building placed successfully!");

        RestoreOriginalMaterials();

        isPlacingBuilding = false;

        originalMaterials.Clear();
        currentMaterials.Clear();
        renderers.Clear();

        currentBuilding = null;
    }

    private void SetBuildingTransparency(Color color)
    {
        foreach (var renderer in renderers)
        {
            var materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = color;
            }
            renderer.materials = materials;
        }
    }

    private void RestoreOriginalMaterials()
    {
        int materialIndex = 0;
        foreach (var renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].CopyPropertiesFromMaterial(originalMaterials[materialIndex]);
                materialIndex++;
            }
            renderer.materials = materials;
        }
    }
}
