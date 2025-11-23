using Sandbox.Rendering;

public sealed class Road : Component
{
    [Property] public GameObject CarPrefab { get; set; }
    [Property] public List<GameObject> Spawners { get; set; } = new();
    [Property] public float MinDelaySpawnCar { get; set; } = 1f;
    [Property] public float MaxDelaySpawnCar { get; set; } = 4f;

    public TimeUntil NextSpawnTime { get; private set; }

    private ObjectPool _carsPool = new(8);
    
    public void SpawnCar()
    {
        if (!CarPrefab.IsValid()) return;

        var randomSpawn = GetRandomSpawnPoint();
        if (!randomSpawn.IsValid()) return;

        var transform = randomSpawn.WorldTransform;
        var pos = randomSpawn.WorldPosition;
        var rot = randomSpawn.WorldRotation;

        var car = _carsPool.Get(CarPrefab, pos, rot).Components.Get<Car>();
        if (!car.IsValid()) return;

        car.WorldPosition = pos;
        car.WorldRotation = rot;

        car.Direction = rot.Forward;

        car.Init();
    }

    public void ResetSpawnTimer(float minDelay, float maxDelay)
    {
        NextSpawnTime = Game.Random.Float(minDelay, maxDelay);
    }

    public void ResetSpawnTimer()
    {
        ResetSpawnTimer(MinDelaySpawnCar, MaxDelaySpawnCar);
    }

    private GameObject GetRandomSpawnPoint()
    {
        return Game.Random.FromList(Spawners);
    }

    protected override void OnStart()
    {
        ResetSpawnTimer();
    }
}