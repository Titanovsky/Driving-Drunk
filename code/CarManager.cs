using Sandbox;
using System.Linq;

public sealed class CarManager : Component
{
    [Property] public GameObject CarPrefab { get; set; }
    [Property] public List<Road> Roads { get; set; } = new();
    [Property] public float DelaySpawnCars { get; set; } = 2f;

    private TimeUntil _timerSpawnCars;

    protected override void OnStart()
	{
        SpawnCar();

        _timerSpawnCars = DelaySpawnCars;
    }

    protected override void OnUpdate()
    {
        if (_timerSpawnCars)
        {
            _timerSpawnCars = DelaySpawnCars;

            SpawnCar();
        }
    }

    public void SpawnCar()
    {
        Transform randomSpawn = Roads.First().GetRandomSpawnPoint();
        Vector3 pos = randomSpawn.Position + new Vector3(0,0,10f); // чуть выше спавнит
        Rotation ang = randomSpawn.Rotation;

        var obj = CarPrefab.Clone(pos, ang);
    }
}
