using System;

public sealed class Road : Component
{
    [Property] public GameObject CarPrefab { get; set; }
    [Property] public List<GameObject> Spawners { get; set; } = new();
    [Property] public float MinDelaySpawnCar { get; set; } = 1f;
    [Property] public float MaxDelaySpawnCar { get; set; } = 4f;

    public TimeUntil NextSpawnTime { get; private set; }

    private GameObjectPool _carsPool = new(8, true);

    public void SpawnCar()
    {
        if (!Networking.IsHost) return;
        if (!CarPrefab.IsValid()) return;

        var randomSpawn = GetRandomSpawnPoint();
        if (!randomSpawn.IsValid()) return;

        var transform = randomSpawn.WorldTransform;
        var pos = randomSpawn.WorldPosition;
        var rot = randomSpawn.WorldRotation;

        //Car car = _carsPool.Get(CarPrefab, new CloneConfig(CarPrefab.WorldTransform, parent: GameObject)).Components.Get<Car>();
        Car car = CarPrefab.Clone(new CloneConfig(CarPrefab.WorldTransform, parent: GameObject)).Components.Get<Car>();
        if (!car.IsValid()) return;

        car.WorldScale = CarPrefab.WorldScale;
        car.WorldPosition = pos;
        car.WorldRotation = rot;
        car.Direction = rot.Forward;

        car.Init();

        car.GameObject.NetworkSpawn();
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
        var seed = (int)(Time.Now * 1000);
        var rng = new Random(seed);

        return Spawners[rng.Next(Spawners.Count)];
    }

    protected override void OnStart()
    {
        ResetSpawnTimer();
    }
}