using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementController : MonoBehaviour
{
    [Header("Building Prefabs")]
    public GameObject townHallPrefab;
    public GameObject barracksPrefab;
    public GameObject warehousePrefab;
    public GameObject marketPrefab;
    public GameObject farmPrefab;
    public GameObject minePrefab;
    public GameObject woodcutterPrefab;
    //public GameObject smallTower;
    //public GameObject mediumTower;
    //public GameObject largeTowerPrefab;
    //public GameObject gatehousePrefab;
     
    private Dictionary<BuildingType, GameObject> buildingPrefabs;

    private LayerMask groundLayers;
    private LayerMask blockingLayers;

    public float buildingRotationSpeed = GlobalSettings.Buildings.RotationSpeed;

    public string PlacementTriggerGameObjectName = "PlacementTrigger";

    public bool IsPlacingBuilding { get; private set; } = false;
    private bool canPlaceBuilding = false;
    private bool couldPlaceBuildingLastFrame = false;

    public Color canBuildColor = new(0, 1, 0, 0.5f);
    public Color cannotBuildColor = new(1, 0, 0, 0.5f);

    private GameObject currentBuilding;
    private List<Renderer> currentBuildingRenderers = new();
    private List<Material> currentBuildingOriginalMaterials = new();
    private List<Material> currentBuildingCurrentMaterials = new();

    private Collider currentBuildingMainCollider;
    private Collider currentBuildingPlacementCollider;

    private ResourceController resourceController;

    private void Start()
    {
        resourceController = GetComponent<ResourceController>();

        groundLayers = LayerManager.GroundLayers;
        blockingLayers = LayerManager.BuildingBlockingLayers;

        buildingPrefabs = new Dictionary<BuildingType, GameObject>
        {
            { BuildingType.TownHall, townHallPrefab },
            { BuildingType.Barracks, barracksPrefab },
            { BuildingType.Warehouse, warehousePrefab },
            { BuildingType.Market, marketPrefab },
            { BuildingType.Farm, farmPrefab },
            { BuildingType.Mine, minePrefab },
            { BuildingType.Woodcutter, woodcutterPrefab },
            //{ BuildingType.SmallTower, smallTower },
            //{ BuildingType.MediumTower, mediumTower },
            //{ BuildingType.LargeTower, largeTowerPrefab },
            //{ BuildingType.Gatehouse, gatehousePrefab }
        };
    }

    private void Update()
    {
        if (IsPlacingBuilding)
        {
            UpdateBuildingPosition();
            CheckPlacementValidity();

            if(couldPlaceBuildingLastFrame != canPlaceBuilding)
            {
                SetBuildingTransparency(canPlaceBuilding ? canBuildColor : cannotBuildColor);
            }

            couldPlaceBuildingLastFrame = canPlaceBuilding;
        }
    }

    public void StartBuildingPlacement(BuildingType buildingType)
    {
        if (!buildingPrefabs.TryGetValue(buildingType, out GameObject buildingPrefab))
        {
            Debug.LogError($"No prefab assigned for building type: {buildingType}");
            return;
        }

        if (!resourceController.CanAfford(buildingPrefab.GetComponent<BuildingBase>().Costs))
        {
            Debug.Log("Can't afford this building.");

            return;
        }

        Debug.Log("Can afford this building.");

        if (currentBuilding != null)
        {
            CancelPlacement();
        }

        IsPlacingBuilding = true;

        currentBuilding = Instantiate(buildingPrefab);

        currentBuildingMainCollider = currentBuilding.GetComponent<Collider>();
        currentBuildingPlacementCollider = currentBuilding.transform.Find(PlacementTriggerGameObjectName).GetComponent<Collider>();

        currentBuildingRenderers.AddRange(currentBuilding.GetComponents<Renderer>());
        currentBuildingRenderers.AddRange(currentBuilding.GetComponentsInChildren<Renderer>());

        foreach (var renderer in currentBuildingRenderers)
        {
            foreach (var material in renderer.materials)
            {
                currentBuildingOriginalMaterials.Add(new Material(material));
                currentBuildingCurrentMaterials.Add(material);
            }
        }

        currentBuildingMainCollider.enabled = false;

        CheckPlacementValidity();

        SetBuildingTransparency(canPlaceBuilding ? canBuildColor : cannotBuildColor);
    }

    public void PlaceBuilding()
    {
        if (!canPlaceBuilding)
        {
            return;
        }

        var building = currentBuilding.GetComponent<BuildingBase>();

        if (!resourceController.SpendResources(building.Costs))
        {
            CancelPlacement();
        }

        RestoreOriginalMaterials();

        currentBuildingMainCollider.enabled = false;

        building.OnBuildingPlaced();

        ClearCurrentBuildingData();

        ClearControllerData();
    }

    public void RotateBuilding(int direction)
    {
        if (currentBuilding != null)
        {
            currentBuilding.transform.Rotate(Vector3.up, direction * buildingRotationSpeed * Time.deltaTime);
        }
    }

    public void CancelPlacement()
    {
        if (currentBuilding != null)
        {
            Destroy(currentBuilding);
        }

        ClearCurrentBuildingData();

        ClearControllerData();
    }

    private void ClearCurrentBuildingData()
    {
        currentBuildingOriginalMaterials.Clear();
        currentBuildingCurrentMaterials.Clear();
        currentBuildingRenderers.Clear();
        currentBuilding = null;
        currentBuildingMainCollider = null;
        currentBuildingPlacementCollider = null;
    }

    private void ClearControllerData()
    {
        IsPlacingBuilding = false;
        canPlaceBuilding = false;
        couldPlaceBuildingLastFrame = false;
    }

    private void UpdateBuildingPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayers))
        {
            currentBuilding.transform.position = hit.point;
        }
    }

    private void CheckPlacementValidity()
    {
        var placementColliderCenter = currentBuildingPlacementCollider.bounds.center;
        var placementColliderExtents = currentBuildingPlacementCollider.bounds.extents;
        var placementColliderRotation = currentBuildingPlacementCollider.transform.rotation;

        currentBuildingPlacementCollider.enabled = false;

        canPlaceBuilding = Physics.OverlapBox(
            placementColliderCenter,
            placementColliderExtents,
            placementColliderRotation,
            blockingLayers)
            .Length == 0;

        currentBuildingPlacementCollider.enabled = true;
    }

    private void SetBuildingTransparency(Color color)
    {
        foreach (var renderer in currentBuildingRenderers)
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

        foreach (var renderer in currentBuildingRenderers)
        {
            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].CopyPropertiesFromMaterial(currentBuildingOriginalMaterials[materialIndex]);
                materialIndex++;
            }

            renderer.materials = materials;
        }
    }
}
