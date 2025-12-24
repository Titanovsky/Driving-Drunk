using Sandbox;

public sealed class Map : Component
{
    [Property] public string Name { get; set; } = "Default";

    [Property, Group("GameObjects")] public GameObject SpawnDirectory { get; set; }
    [Property, Group("GameObjects")] public GameObject RoadDirectory { get; set; }
    [Property, Group("GameObjects")] public GameObject FinishCollider { get; set; }
}
