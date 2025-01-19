using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingPlacementController : MonoBehaviour
{
    // ---------- GENERAL -------------------
    [Header("Building Prefabs")]
    public GameObject townHallPrefab;
    public GameObject barracksPrefab;
    public GameObject warehousePrefab;
    public GameObject marketPrefab;
    public GameObject farmPrefab;
    public GameObject minePrefab;
    public GameObject woodcutterPrefab;
    public GameObject smallTower;
    public GameObject mediumTowerPrefab;
    public GameObject largeTowerPrefab;
    public GameObject gatehousePrefab;
    public GameObject wallSectionPrefab;

    private Dictionary<BuildingType, GameObject> buildingTypesAndPrefabs;

    public float buildingRotationSpeed = GlobalSettings.Buildings.RotationSpeed;

    public string PlacementTriggerGameObjectName = "PlacementTrigger";

    private LayerMask groundLayers;
    private LayerMask blockingLayers;

    public Color canBuildColor = new(0, 1, 0, 0.5f);
    public Color cannotBuildColor = new(1, 0, 0, 0.5f);

    private ResourceController resourceController;
    private PathfindingGrid pathfindingGrid;

    // --------- BUILDING RELATED -----------
    public bool IsPlacingBuilding { get; private set; } = false;
    private bool canPlaceBuilding = false;
    private bool couldPlaceBuildingLastFrame = false;

    private GameObject currentBuilding;
    private BuildingBase currentBuildingBase;
    private List<Renderer> currentBuildingRenderers = new();
    private List<Material> currentBuildingOriginalMaterials = new();
    private List<Material> currentBuildingCurrentMaterials = new();
    private List<BuildingType> currentBuildingAllowedCollisionBuildingTypes = new();

    private List<Collider> currentBuildingPlacementColliders = new();

    // ---------- WALL RELATED --------------
    public bool IsLookingForWallPlacementLocation { get; private set; } = false;
    private bool canPlaceWall = false;
    private bool couldPlaceWallLastFrame = false;

    public bool IsPlacingWalls = false;
    private bool canPlaceWalls = false;
    private bool couldPlaceWallsLastFrame = false;

    private float WallGridSize = 0.8f;

    private BuildingBase wallBuildingBase;
    private List<BuildingType> wallAllowedCollisionBuildingTypes = new();

    private GameObject placementCheckWallSection;
    private Collider placementCheckWallSectionPlacementCollider;
    private List<Renderer> placementCheckWallSectionRenderers = new();
    private Vector3 currentPlacementCheckWallSectionLocation;
    private Vector3 lastPlacementCheckWallSectionLocation;

    private List<Material> wallOriginalMaterials = new();

    private List<GameObject> walls = new();
    private List<Collider> wallPlacementColliders = new();
    private List<Renderer> wallRenderers = new();

    private Vector3 dragStartPosition;
    private Vector3 currentDragEndPosition;
    private Vector3 lastDragEndPosition;

    private void Start()
    {
        resourceController = GetComponent<ResourceController>();
        pathfindingGrid = GetComponent<PathfindingGrid>();

        groundLayers = LayerManager.GroundLayers;
        blockingLayers = LayerManager.BuildingBlockingLayers;

        buildingTypesAndPrefabs = new Dictionary<BuildingType, GameObject>
        {
            { BuildingType.TownHall, townHallPrefab },
            { BuildingType.Barracks, barracksPrefab },
            { BuildingType.Warehouse, warehousePrefab },
            { BuildingType.Market, marketPrefab },
            { BuildingType.Farm, farmPrefab },
            { BuildingType.Mine, minePrefab },
            { BuildingType.Woodcutter, woodcutterPrefab },
            { BuildingType.SmallTower, smallTower },
            { BuildingType.MediumTower, mediumTowerPrefab },
            { BuildingType.LargeTower, largeTowerPrefab },
            { BuildingType.Gatehouse, gatehousePrefab },
            { BuildingType.Walls, wallSectionPrefab },
        };
    }

    private void Update()
    {
        if (IsPlacingBuilding)
        {
            HandleBuildingPlacement();
        }
        else if (IsLookingForWallPlacementLocation && !IsPlacingWalls)
        {
            HandleLookingForWallPlacementLocation();
        }
        else if (IsPlacingWalls)
        {
            HandleWallPlacement();
        }
    }

    /* ------ BUILDING PLACEMENT LOGIC ------ */

    public bool TryGetBuildingPrefab(BuildingType buildingType, out GameObject buildingPrefab)
    {
        return buildingTypesAndPrefabs.TryGetValue(buildingType, out buildingPrefab);
    }

    public void StartBuildingPlacement(BuildingType buildingType)
    {
        if (!buildingTypesAndPrefabs.TryGetValue(buildingType, out var buildingPrefab))
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

        if (placementCheckWallSection != null || currentBuilding != null)
        {
            CancelPlacement();
        }

        IsPlacingBuilding = true;

        currentBuilding = Instantiate(buildingPrefab);

        currentBuildingPlacementColliders = currentBuilding.transform.Find(PlacementTriggerGameObjectName).GetComponents<Collider>().ToList();

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

        currentBuildingBase = currentBuilding.GetComponent<BuildingBase>();
        currentBuildingAllowedCollisionBuildingTypes.AddRange(currentBuildingBase.AllowedCollisionBuildingTypes);

        CheckBuildingPlacementValidity();

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

        UpdatePathfindingGridForBuilding();

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

        DestroyWallGameObjects();

        ClearAllWallData();

        ClearControllerData();
    }

    private void HandleBuildingPlacement()
    {
        UpdateBuildingPosition();
        CheckBuildingPlacementValidity();

        if (couldPlaceBuildingLastFrame != canPlaceBuilding)
        {
            SetBuildingTransparency(canPlaceBuilding ? canBuildColor : cannotBuildColor);
        }

        couldPlaceBuildingLastFrame = canPlaceBuilding;
    }

    private void ClearCurrentBuildingData()
    {
        currentBuildingOriginalMaterials.Clear();
        currentBuildingCurrentMaterials.Clear();
        currentBuildingRenderers.Clear();
        currentBuildingAllowedCollisionBuildingTypes.Clear();
        currentBuildingPlacementColliders.Clear();
        currentBuildingBase = null;
        currentBuilding = null;
    }

    private void ClearControllerData()
    {
        IsPlacingBuilding = false;
        canPlaceBuilding = false;
        couldPlaceBuildingLastFrame = false;

        IsLookingForWallPlacementLocation = false;
        canPlaceWall = false;
        couldPlaceWallLastFrame = false;

        IsPlacingWalls = false;
        canPlaceWalls = false;
        couldPlaceWallsLastFrame = false;
    }

    private void UpdateBuildingPosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayers))
        {
            currentBuilding.transform.position = hit.point;
        }
    }

    private void CheckBuildingPlacementValidity()
    {
        var buildingPlacementIsValid = true;

        foreach (var currentBuildingPlacementCollider in currentBuildingPlacementColliders)
        {
            if (!buildingPlacementIsValid)
            {
                break;
            }

            var boxCollider = currentBuildingPlacementCollider as BoxCollider;
            if (boxCollider == null)
            {
                continue;
            }

            var boxCenter = boxCollider.transform.TransformPoint(boxCollider.center);
            var boxSize = boxCollider.size / 2f;
            var boxRotation = boxCollider.transform.rotation;

            currentBuildingPlacementCollider.enabled = false;

            var collidingObjectColliders = Physics.OverlapBox(
                boxCenter,
                boxSize,
                boxRotation,
                blockingLayers);

            if (collidingObjectColliders.Length > 0)
            {
                foreach (var collider in collidingObjectColliders)
                {
                    if (!buildingPlacementIsValid)
                    {
                        break;
                    }

                    var collidingBuilding = collider.GetComponentInParent<BuildingBase>();

                    buildingPlacementIsValid =
                        collidingBuilding != null &&
                        currentBuildingAllowedCollisionBuildingTypes.Contains(collidingBuilding.BuildingType);
                }
            }

            currentBuildingPlacementCollider.enabled = true;
        }

        canPlaceBuilding = buildingPlacementIsValid;
    }

    private void SetBuildingTransparency(Color color)
    {
        foreach (var renderer in currentBuildingRenderers)
        {
            var materials = renderer.materials;

            foreach (var material in materials)
            {
                material.color = color;
            }

            renderer.materials = materials;
        }
    }

    private void RestoreOriginalMaterials()
    {
        int materialIndex = 0;

        foreach (var renderer in currentBuildingRenderers)
        {
            var materials = renderer.materials;

            foreach (var material in materials)
            {
                material.CopyPropertiesFromMaterial(currentBuildingOriginalMaterials[materialIndex]);
                materialIndex++;
            }

            renderer.materials = materials;
        }
    }

    private void UpdatePathfindingGridForBuilding()
    {
        var pathfindingGrid = FindObjectOfType<PathfindingGrid>();

        foreach (var currentBuildingPlacementCollider in currentBuildingPlacementColliders)
        {
            var boxCollider = currentBuildingPlacementCollider as BoxCollider;

            if (boxCollider == null)
            {
                continue;
            }

            pathfindingGrid.UpdateNodesForBuilding(boxCollider, false);
        }
    }

    /* --------------------------------------------- */
    /* ---- LOOKING FOR WALL PLACEMENT LOCATION ---- */

    public void StartLookingForWallPlacementLocation()
    {
        if (!buildingTypesAndPrefabs.TryGetValue(BuildingType.Walls, out var wallPrefab))
        {
            Debug.LogError($"No prefab assigned for building type: {BuildingType.Walls}");
            return;
        }

        if (!resourceController.CanAfford(wallPrefab.GetComponent<BuildingBase>().Costs))
        {
            Debug.Log("Can't afford walls.");

            return;
        }

        Debug.Log("Can afford walls.");

        if (placementCheckWallSection != null || currentBuilding != null)
        {
            CancelPlacement();
        }

        IsLookingForWallPlacementLocation = true;

        placementCheckWallSection = Instantiate(wallPrefab);

        placementCheckWallSectionPlacementCollider = placementCheckWallSection.transform.Find(PlacementTriggerGameObjectName).GetComponent<Collider>();

        placementCheckWallSectionRenderers.AddRange(placementCheckWallSection.GetComponents<Renderer>());
        placementCheckWallSectionRenderers.AddRange(placementCheckWallSection.GetComponentsInChildren<Renderer>());

        wallBuildingBase = placementCheckWallSection.GetComponent<BuildingBase>();
        wallAllowedCollisionBuildingTypes.AddRange(wallBuildingBase.AllowedCollisionBuildingTypes);

        foreach (var renderer in placementCheckWallSectionRenderers)
        {
            foreach (var material in renderer.materials)
            {
                wallOriginalMaterials.Add(new Material(material));
            }
        }
        
        CheckCurrentWallPlacementValidity();

        SetBuildingTransparency(canPlaceBuilding ? canBuildColor : cannotBuildColor);
    }

    private void CheckCurrentWallPlacementValidity()
    {
        var placementColliderCenter = placementCheckWallSectionPlacementCollider.bounds.center;
        var placementColliderExtents = placementCheckWallSectionPlacementCollider.bounds.extents;
        var placementColliderRotation = placementCheckWallSectionPlacementCollider.transform.rotation;

        placementCheckWallSectionPlacementCollider.enabled = false;

        canPlaceWall = true;

        var collidingObjectColliders = Physics.OverlapBox(
            placementColliderCenter,
            placementColliderExtents,
            placementColliderRotation,
            blockingLayers);

        if (collidingObjectColliders.Length > 0)
        {
            foreach (var collider in collidingObjectColliders)
            {
                if (!canPlaceWall)
                {
                    break;
                }

                var collidingBuilding = collider.GetComponentInParent<BuildingBase>();

                canPlaceWall = collidingBuilding != null && wallAllowedCollisionBuildingTypes.Contains(collidingBuilding.BuildingType);
            }
        }

        placementCheckWallSectionPlacementCollider.enabled = true;
    }

    private void SetCurrentWallTransparency(Color color)
    {
        foreach (var renderer in placementCheckWallSectionRenderers)
        {
            var materials = renderer.materials;

            foreach (var material in materials)
            {
                material.color = color;
            }

            renderer.materials = materials;
        }
    }

    private void HandleLookingForWallPlacementLocation()
    {
        UpdateCurrentWallPosition();
        CheckCurrentWallPlacementValidity();

        if (couldPlaceWallLastFrame != canPlaceWall)
        {
            SetCurrentWallTransparency(canPlaceWall ? canBuildColor : cannotBuildColor);
        }

        couldPlaceWallLastFrame = canPlaceWall;
    }

    private void UpdateCurrentWallPosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayers))
        {
            currentPlacementCheckWallSectionLocation = SnapToGrid(hit.point);
            placementCheckWallSection.transform.position = currentPlacementCheckWallSectionLocation;

            if(lastPlacementCheckWallSectionLocation != currentPlacementCheckWallSectionLocation)
            {
                var direction = (lastPlacementCheckWallSectionLocation - currentPlacementCheckWallSectionLocation).normalized;
                placementCheckWallSection.transform.forward = direction;

                lastPlacementCheckWallSectionLocation = currentPlacementCheckWallSectionLocation;
            }
        }
    }

    /* --------------------------------------------- */
    /* -------- WALL PLACEMENT VIA DRAGING --------- */

    public void StartWallPlacement()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity, groundLayers))
        {
            if(placementCheckWallSection != null)
            {
                Destroy(placementCheckWallSection);
            }

            ClearCurrentWallData();

            dragStartPosition = SnapToGrid(hit.point);

            IsLookingForWallPlacementLocation = false;
            IsPlacingWalls = true;
        }
    }

    private Vector3 SnapToGrid(Vector3 position)
    {
        float snappedX = Mathf.Round(position.x / WallGridSize) * WallGridSize;
        float snappedZ = Mathf.Round(position.z / WallGridSize) * WallGridSize;

        return new Vector3(snappedX, position.y, snappedZ);
    }

    private void HandleWallPlacement()
    {
        UpdateDragEndPosition();

        if(currentDragEndPosition == lastDragEndPosition)
        {
            return;
        }

        GetWallChainBetweenStartAndCurrentPosition();

        CheckWallChainPlacementValidity();

        if (canPlaceWalls != couldPlaceWallsLastFrame)
        {
            SetWallChainTransparency(canPlaceWalls ? canBuildColor : cannotBuildColor);
        }

        ClearNullsFromWallLists();

        lastDragEndPosition = currentDragEndPosition;
        couldPlaceWallsLastFrame = canPlaceWalls;
    }

    private void UpdateDragEndPosition()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity, groundLayers))
        {
            currentDragEndPosition = SnapToGrid(hit.point);
        }
    }

    private void GetWallChainBetweenStartAndCurrentPosition()
    {
        var direction = (currentDragEndPosition - dragStartPosition).normalized;
        var distance = Vector3.Distance(dragStartPosition, currentDragEndPosition);

        var neededWallSectionCount = Mathf.FloorToInt(distance / WallGridSize);
        neededWallSectionCount = neededWallSectionCount == 0 ? 1 : neededWallSectionCount;
        var currentWallSectionCount = walls.Count;

        if (currentWallSectionCount < neededWallSectionCount)
        {
            for (int i = currentWallSectionCount; i <= neededWallSectionCount; i++)
            {
                var wall = Instantiate(wallSectionPrefab, dragStartPosition, Quaternion.identity);
                walls.Add(wall);

                var thisWallPlacementCollider = wall.transform.Find(PlacementTriggerGameObjectName).GetComponent<Collider>();
                thisWallPlacementCollider.enabled = false;
                wallPlacementColliders.Add(thisWallPlacementCollider);

                var thisWallRenderers = wall.GetComponentsInChildren<Renderer>();
                wallRenderers.AddRange(thisWallRenderers);

                foreach (var renderer in thisWallRenderers)
                {
                    foreach (var material in renderer.materials)
                    {
                        wallOriginalMaterials.Add(new Material(material));

                        material.color = couldPlaceWallsLastFrame ? canBuildColor : cannotBuildColor;
                    }
                }
            }
        }
        else if (currentWallSectionCount > neededWallSectionCount)
        {
            for (int i = currentWallSectionCount - 1; i >= neededWallSectionCount; i--)
            {
                var wallSectionToBeDestroyed = walls[i];

                walls.RemoveAt(i);

                Destroy(wallSectionToBeDestroyed);
            }
        }

        Debug.Log($"start pos: {dragStartPosition}, end pos: {currentDragEndPosition}, direction: {direction}, distance: {distance}, needed section count: {neededWallSectionCount}, current section count: {currentWallSectionCount}");

        var positionIndex = 0;

        foreach (var wall in walls)
        {
            var position = dragStartPosition + positionIndex * WallGridSize * direction;

            wall.transform.position = position;
            wall.transform.forward = direction;

            positionIndex++;
        }

        lastDragEndPosition = currentDragEndPosition;
    }
    
    private void CheckWallChainPlacementValidity()
    {
        var canPlaceWallChain = true;

        foreach (var wallPlacementCollider in wallPlacementColliders.Where(x => x != null))
        {
            if (!canPlaceWallChain)
            {
                break;
            }

            var boxCollider = wallPlacementCollider as BoxCollider;
            if (boxCollider == null)
            {
                continue;
            }

            var boxCenter = boxCollider.transform.TransformPoint(boxCollider.center);
            var boxSize = boxCollider.size / 2f;
            var boxRotation = boxCollider.transform.rotation;

            wallPlacementCollider.enabled = false;

            var collidingObjectColliders = Physics.OverlapBox(
                boxCenter,
                boxSize,
                boxRotation,
                blockingLayers);

            if (collidingObjectColliders.Length > 0)
            {
                foreach (var collider in collidingObjectColliders)
                {
                    if (!canPlaceWallChain)
                    {
                        break;
                    }

                    var collidingBuilding = collider.GetComponentInParent<BuildingBase>();

                    canPlaceWallChain =
                        collidingBuilding != null &&
                        wallAllowedCollisionBuildingTypes.Contains(collidingBuilding.BuildingType);
                }
            }

            wallPlacementCollider.enabled = true;
        }

        canPlaceWalls = canPlaceWallChain;
    }

    private void SetWallChainTransparency(Color color)
    {
        foreach (var renderer in wallRenderers.Where(x => x != null))
        {
            var materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = color;
            }

            renderer.materials = materials;
        }
    }

    private void ClearNullsFromWallLists()
    {
        wallPlacementColliders.RemoveAll(x => x == null);

        wallRenderers.RemoveAll(x => x == null);

        wallOriginalMaterials.RemoveAll(x => x == null);
    }

    public void PlaceWallChain()
    {
        if (walls.Count == 0
            || !canPlaceWalls
            || !SpendResourcesForWallChainPlacement())
        {
            CancelPlacement();
        }

        RestoreWallChainOriginalMaterials();

        ReenableWallChainPlacementCollidersAndUpdatePathfindingGrid();

        ClearAllWallData();

        ClearControllerData();
    }

    private bool SpendResourcesForWallChainPlacement()
    {
        var wallSectionCosts = wallSectionPrefab.GetComponent<BuildingBase>().Costs;
        var totalCosts = new List<Cost>();

        foreach (var cost in wallSectionCosts)
        {
            totalCosts.Add(new Cost
            {
                resourceType = cost.resourceType,
                amount = cost.amount * walls.Count
            });
        }

        return resourceController.SpendResources(totalCosts.ToArray());
    }

    private void RestoreWallChainOriginalMaterials()
    {
        foreach (var renderer in wallRenderers.Where(x => x != null))
        {
            var materials = renderer.materials;
            int materialIndex = 0;

            foreach (var material in materials)
            {
                material.CopyPropertiesFromMaterial(wallOriginalMaterials[materialIndex]);
                materialIndex++;
            }

            renderer.materials = materials;
        }
    }

    private void ReenableWallChainPlacementCollidersAndUpdatePathfindingGrid()
    {
        var pathfindingGrid = FindObjectOfType<PathfindingGrid>();

        foreach (var placementCollider in wallPlacementColliders.Where(x => x != null))
        {
            placementCollider.enabled = true;

            var boxCollider = placementCollider as BoxCollider;

            if (boxCollider == null)
            {
                continue;
            }

            pathfindingGrid.UpdateNodesForBuilding(boxCollider, false);
        }
    }

    private void ClearCurrentWallData()
    {
        placementCheckWallSection = null;
        placementCheckWallSectionPlacementCollider = null;
        placementCheckWallSectionRenderers.Clear();
    }

    private void ClearWallData()
    {
        wallBuildingBase = null;
        wallAllowedCollisionBuildingTypes.Clear();
        walls.Clear();
        wallPlacementColliders.Clear();
        wallRenderers.Clear();
        wallOriginalMaterials.Clear();
    }

    public void ClearAllWallData()
    {
        ClearCurrentWallData();
        ClearWallData();
    }

    private void DestroyWallGameObjects()
    {
        if (placementCheckWallSection != null)
        {
            Destroy(placementCheckWallSection);
        }

        foreach(var wall in walls)
        {
            Destroy(wall);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    if (currentBuildingPlacementColliders != null)
    //    {
    //        foreach (var collider in currentBuildingPlacementColliders)
    //        {
    //            Gizmos.color = Color.red;

    //            var boxCollider = collider as BoxCollider;

    //            if (boxCollider != null)
    //            {
    //                var matrix = Matrix4x4.TRS(
    //                    boxCollider.transform.position,
    //                    boxCollider.transform.rotation,
    //                    Vector3.one);

    //                Gizmos.matrix = matrix;
    //                Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
    //            }
    //        }
    //    }
    //}
}
