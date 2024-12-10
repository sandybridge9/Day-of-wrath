using UnityEngine;

public static class LayerManager
{
    public static LayerMask SelectableLayers => LayerMask.GetMask(
        GlobalSettings.Layers.UnitLayer,
        GlobalSettings.Layers.BuildingLayer
    );

    public static LayerMask GroundLayers => LayerMask.GetMask(GlobalSettings.Layers.TerrainLayer);

    public static LayerMask WalkableLayers => LayerMask.GetMask(GlobalSettings.Layers.TerrainLayer);

    public static LayerMask BuildingBlockingLayers => LayerMask.GetMask(
        GlobalSettings.Layers.UnitLayer,
        GlobalSettings.Layers.BuildingLayer
        //GlobalSettings.Layers.TerrainLayer
    );

    public static LayerMask UnitLayer => LayerMask.GetMask(GlobalSettings.Layers.UnitLayer);
}
