public sealed class CarManager : Component
{
    [Property] public List<Road> Roads { get; set; } = new();
    [Property] public GameObject RoadsDirectory { get; set; }

    public void CollectRoads()
    {
        if (!RoadsDirectory.IsValid()) return;

        Roads.Clear();

        foreach (var gameObj in RoadsDirectory.Children)
        {
            if (!gameObj.Components.TryGet<Road>(out var road)) continue;
            if (Roads.Contains(road)) continue;

            Roads.Add(road);
        }
    }

    private void Loop()
    {
        if (!Networking.IsHost) return;

        foreach (var road in Roads)
        {
            if (!road.IsValid()) continue;
            if (!road.NextSpawnTime) continue;

            road.ResetSpawnTimer();
            road.SpawnCar();
        }
    }

    protected override void OnStart()
    {
        CollectRoads();
    }
    
    protected override void OnUpdate()
    {
        Loop();
    }
}
