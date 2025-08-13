using Sandbox;
using Sandbox.Services;
using System.Threading.Tasks;

public sealed class Player : Component
{
    [Property] public PlayerController controller;
    [Property] public ModelRenderer body;
    [Property] public HudWorld HudWorld;
    [Property] public float speed = 10f;

    public bool IsAlive { get; set; } = true;

    private GameObject _corp;
    private Transform _transformRespawn;

    public void Die(Car killer)
    {
        if (!IsAlive) return;
        IsAlive = false;

        controller.ColliderObject.Enabled = false;

        _corp = controller.CreateRagdoll($"Corp: {Connection.Local.DisplayName}");
        var rb = _corp.GetComponentInChildren<Rigidbody>();

        var direction = killer.WorldRotation.Forward;
        rb.Velocity *= direction * killer.Speed * 100f;

        body.GameObject.Enabled = false;
        controller.Enabled = false;
        HudWorld.Enabled = false;

        Log.Info("Die");

        _ = RespawnAsync(2.4f);
    }

    private async Task RespawnAsync(float seconds)
    {
        await Task.DelaySeconds(seconds);

        Respawn();
    }

    public void Respawn()
    {
        if (_corp.IsValid())
            _corp.Destroy();

        WorldPosition = _transformRespawn.Position;
        WorldRotation = _transformRespawn.Rotation;

        controller.ColliderObject.Enabled = true;
        body.GameObject.Enabled = true;
        HudWorld.Enabled = true;
        controller.Enabled = true;

        IsAlive = true;

        Log.Info("Respawn");
    }

    private void UpdateTeleportToCorp()
    {
        if (IsAlive && !_corp.IsValid()) return;

        WorldPosition = _corp.WorldPosition;
    }

    protected override void OnStart()
    {
        _transformRespawn = WorldTransform;

        HudWorld.Name = Connection.Local.DisplayName;
    }

    protected override void OnUpdate()
    {
        UpdateTeleportToCorp();
    }
}
