using Sandbox;

public sealed class Road : Component
{
    [Property] public List<GameObject> Spawners { get; set; } = new();

    public Transform GetRandomSpawnPoint()
    {
        return Game.Random.FromList(Spawners).WorldTransform;
    }
}
