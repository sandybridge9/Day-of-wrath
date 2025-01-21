using UnityEngine;

public static class LayerManager
{
    public static LayerMask SelectableLayers => LayerMask.GetMask(
        GlobalSettings.Layers.UnitLayer,
        GlobalSettings.Layers.BuildingLayer
    );

    public static LayerMask GroundLayers => LayerMask.GetMask(GlobalSettings.Layers.TerrainLayer);

    public static LayerMask WalkableLayers => LayerMask.GetMask(GlobalSettings.Layers.TerrainLayer);

    public static LayerMask UnitLayer => LayerMask.GetMask(GlobalSettings.Layers.UnitLayer);

    public static LayerMask BuildingLayer = LayerMask.GetMask(GlobalSettings.Layers.BuildingLayer);

    public static LayerMask BuildingBlockingLayers => LayerMask.GetMask(
        GlobalSettings.Layers.UnitLayer,
        GlobalSettings.Layers.BuildingLayer
        //GlobalSettings.Layers.TerrainLayer
    );

    public static LayerMask UnitTrainingBlockingLayers => LayerMask.GetMask(
        GlobalSettings.Layers.UnitLayer,
        GlobalSettings.Layers.BuildingLayer
    );

    public static LayerMask UnitMovementObstacleLayers => LayerMask.GetMask(GlobalSettings.Layers.BuildingLayer);

    public static LayerMask AttackableLayers => LayerMask.GetMask(
        GlobalSettings.Layers.UnitLayer,
        GlobalSettings.Layers.BuildingLayer
    );

    public static LayerMask PathfindingGridUnwalkableLayers => LayerMask.GetMask(
        GlobalSettings.Layers.ResourceLayer,
        GlobalSettings.Layers.BuildingLayer);

    public static LayerMask ResourceSpawningBlockingLayers => LayerMask.GetMask(
        GlobalSettings.Layers.ResourceLayer,
        GlobalSettings.Layers.BuildingLayer);
}
