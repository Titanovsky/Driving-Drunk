using Sandbox;
using System.Linq;

public sealed class CarManager : Component
{
    [Property] public GameObject CarPrefab { get; set; }
    [Property] public List<Road> Roads { get; set; } = new();
    [Property] public float MinDelaySpawnCar { get; set; } = 1f;
    [Property] public float MaxDelaySpawnCar { get; set; } = 4f;

    private TimeUntil _timerSpawnCars;

    protected override void OnStart()
	{
        foreach (var road in Scene.GetAll<Road>())
            Roads.Add(road);

        ResetTimer();
    }

    protected override void OnUpdate()
    {
        if (_timerSpawnCars)
        {
            ResetTimer();

            SpawnCar();
        }
    }

    private void ResetTimer()
    {
        _timerSpawnCars = Game.Random.Int((int) MinDelaySpawnCar, (int) MaxDelaySpawnCar);
    }

    public void SpawnCar()
    {
        foreach (var road in Roads)
        {
            Transform randomSpawn = road.GetRandomSpawnPoint();
            Vector3 pos = randomSpawn.Position + new Vector3(0, 0, 10f); // чуть выше спавнит
            Rotation ang = randomSpawn.Rotation;

            var obj = CarPrefab.Clone(pos, ang);
        }
    }
}
