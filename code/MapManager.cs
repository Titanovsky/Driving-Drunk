using System;

public sealed class MapManager : Component
{
    public static MapManager Instance { get; private set; }

    private GameObject FinishLineCollider { get; set; }
    private float _minPosForLine = 24f;

    [Property] public CarManager CarManager { get; set; }
    [Property] public List<Map> Maps { get; set; } = new();
    public int MapIndex { get; private set; } = 0;

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

        NextMapRpc();
        SetupMapRpc();
        RespawnAll();
    }

    [Rpc.Broadcast]
    private void NextMapRpc()
    {
        if (!Rpc.Caller.IsHost) return;

        NextMap();
    }

    [Rpc.Owner]
    private void SyncMapIndex(int index)
    {
        using (Rpc.FilterInclude(connect => !connect.IsHost))
        {
            if (!Rpc.Caller.IsHost) return;

            MapIndex = index;

            Log.Info("Map Index sync from Host");

            SetupMap();
        }
    }

    private void NextMap()
    {
        MapIndex = MapIndex + 1;

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

        var currentMap = Maps[MapIndex];

        CarManager.ClearCars();
        Maps[previousMapIndex].GameObject.Enabled = false;

        currentMap.GameObject.Enabled = true;
        CarManager.RoadsDirectory = currentMap.RoadDirectory;
        CarManager.CollectRoads();

        SpawnPickups();

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

    [Rpc.Broadcast]
    private void SpawnPickups()
    {
        if (!Rpc.Caller.IsHost) return;

        Map currentMap = Maps[MapIndex];

        foreach (var pickup in Scene.GetAllComponents<PickUp>())
            pickup.GameObject.Destroy();


    }

    [Rpc.Host]
    private void RequestSyncMapIndex()
    {
        CarManager.ClearCars();
        SyncMapIndex(MapIndex);
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
        if (!IsProxy)
        {
            Log.Info("Send Request Sync Map Index to Host");
            RequestSyncMapIndex();
        }
    }
}
