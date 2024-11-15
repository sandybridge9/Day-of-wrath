public static class GlobalSettings
{
    public static class Layers
    {
        public const string TerrainLayer = "Terrain";

        public const string UnitLayer = "Unit";

        public const string BuildingLayer = "Building";
    }

    public static class MouseInput
    {
        public const float LeftClickMouseMovementDistanceThreshold = 1f;

        public const float CameraRotationDistanceThreshold = 40f;
    }

    public static class Camera
    {
        public const float MovementSpeed = 10.0f;
        public const float ZoomSpeed = 10.0f;
        public const float RotationSpeed = 1.0f;
        public const float MaxYRotationAngle = 80f;
    }

    public static class Buildings
    {
        public const float RotationSpeed = 90f;
    }
}