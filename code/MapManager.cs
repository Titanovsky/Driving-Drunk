using Sandbox;

public sealed class MapManager : Component
{
    public static MapManager Instance { get; set; }

    [Property] public GameObject FinishLineCollider { get; set; }
    private float _minPosForLine = 24f;

    public bool CheckFinish(Player ply)
    {
        BoxCollider collider = FinishLineCollider.Components.Get<BoxCollider>();

        var diff = Vector3.DistanceBetween(collider.GetWorldBounds().ClosestPoint(ply.body.WorldPosition), ply.body.WorldPosition);
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
            if (newConnect.SteamId != connect.SteamId) continue;
            if (!ply.IsAlive) continue;

            if (CheckFinish(ply))
                Finish();
        }
    }

    private void Finish(Player ply = null)
    {
        if (!Networking.IsHost) return;

        Log.Info("Finished");
    }

    protected override void OnAwake()
	{
        Instance = this;
	}

    protected override void OnDestroy()
    {
        Instance = null;
    }
}
