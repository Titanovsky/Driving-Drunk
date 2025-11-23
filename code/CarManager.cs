public sealed class CarManager : Component
{
    [Property] public List<Road> Roads { get; set; } = new();
    [Property] public GameObject RoadsDirectory { get; set; }

    private void CollectRoads()
    {
        if (!RoadsDirectory.IsValid()) return;

        foreach (var gameObj in RoadsDirectory.Children)
        {
            if (!gameObj.Components.TryGet<Road>(out var road)) continue;
            if (Roads.Contains(road)) continue;

            Roads.Add(road);
        }
    }

    protected override void OnStart()
    {
        CollectRoads();
    }
    
    protected override void OnUpdate()
    {
        foreach (var road in Roads)
        {
            if (!road.IsValid()) continue;
            if (!road.NextSpawnTime) continue;

            road.SpawnCar();
        }
    }
}
