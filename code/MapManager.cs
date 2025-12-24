using System;

public sealed class MapManager : Component
{
    public static MapManager Instance { get; private set; }

    private GameObject FinishLineCollider { get; set; }
    private float _minPosForLine = 24f;

    [Property] public CarManager CarManager { get; set; }
    [Property] public List<Map> Maps { get; set; } = new();
    [Sync(SyncFlags.FromHost)] public int MapIndex { get; private set; } = 0;

    public bool CheckFinish(Player ply)
    {
        BoxCollider collider = FinishLineCollider.Components.Get<BoxCollider>();

        var diff = Vector3.DistanceBetween(collider.GetWorldBounds().ClosestPoint(ply.body.WorldPosition), ply.body.WorldPosition);

        Log.Info($"Check Finish: {diff}");
        return diff <= _minPosForLine;
    }

    [Rpc.Host]
    public void SendFinishToHost()
    {
        var connect = Rpc.Caller;
        if (!connect.IsActive) return;

        foreach (var ply in Scene.GetAllComponents<Player>())
        {
            var newConnect = ply.Network.Owner;
            if (newConnect != connect) continue;
            if (!ply.IsAlive) continue;

            Finish();

            //if (CheckFinish(ply))
            //    Finish();
            //else
            //    connect.Kick("suspicion of cheating [Finish]"); //? should be?
        }
    }

    private void Finish()
    {
        if (!Networking.IsHost) return;

        Log.Info("Finished");

        NextMap();
        SetupMapRpc();
    }

    private void NextMap()
    {
        if (!Networking.IsHost) return;

        MapIndex++;

        MapIndex = (MapIndex >= Maps.Count) ? 0 : MapIndex;
    }

    [Rpc.Broadcast]
    private void SetupMapRpc()
    {
        if (!Rpc.Caller.IsHost) return;

        SetupMap();
    }

    private void SetupMap()
    {
        Log.Info("SetupMap");

        var previousMapIndex = (MapIndex == 0) ? (Maps.Count - 1) : MapIndex - 1;

        Log.Info(previousMapIndex);

        RespawnAll();

        var currentMap = Maps[MapIndex];
        Maps[previousMapIndex].GameObject.Enabled = false;
        currentMap.GameObject.Enabled = true;

        CarManager.RoadsDirectory = currentMap.RoadDirectory;
        CarManager.CollectRoads();

        FinishLineCollider = currentMap.FinishCollider;
    }

    [Rpc.Broadcast]
    private void RespawnAll()
    {
        if (!Rpc.Caller.IsHost) return;

        foreach (var ply in Scene.GetAllComponents<Player>())
        {
            if (!ply.IsAlive) continue;

            ply.Respawn();
        }

    }

    private void CreateSingleton()
    {
        if (Instance != null) return;

        Instance = this;
    }

    private void RemoveSingleton()
    {
        if (Instance == null) return;

        Instance = null;
    }

    protected override void OnAwake()
	{
        CreateSingleton();

    }

    protected override void OnDestroy()
    {
        RemoveSingleton();
    }

    protected override void OnStart()
    {
        SetupMap();
    }
}
