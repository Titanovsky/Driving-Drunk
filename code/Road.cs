public sealed class Road : Component
{
    [Property] public GameObject CarPrefab { get; set; }
    [Property] public List<GameObject> Spawners { get; set; } = new();
    [Property] public float MinDelaySpawnCar { get; set; } = 1f;
    [Property] public float MaxDelaySpawnCar { get; set; } = 4f;

    public TimeUntil NextSpawnTime { get; private set; }

    public void SpawnCar()
    {
        if (!CarPrefab.IsValid()) return;

        var randomSpawn = GetRandomSpawnPoint();
        if (!randomSpawn.IsValid) return;

        var pos = randomSpawn.Position;
        var rot = randomSpawn.Rotation;

        CarPrefab.Clone(pos, rot);
    }

    public void ResetSpawnTimer(float minDelay, float maxDelay)
    {
        NextSpawnTime = Game.Random.Float(minDelay, maxDelay);
    }

    public void ResetSpawnTimer()
    {
        ResetSpawnTimer(MinDelaySpawnCar, MaxDelaySpawnCar);
    }

    private Transform GetRandomSpawnPoint()
    {
        return Game.Random.FromList(Spawners).WorldTransform;
    }

    protected override void OnStart()
    {
        ResetSpawnTimer();
    }
}