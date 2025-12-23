using Sandbox;

public sealed class Map : Component
{
    [Property] public string Name { get; set; } = "Default";

    [Property, Group("GameObjects")] public GameObject SpawnDirectory { get; set; } = new();
    [Property, Group("GameObjects")] public GameObject RoadDirectory { get; set; } = new();
}
