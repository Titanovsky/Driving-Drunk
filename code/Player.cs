using Sandbox;
using Sandbox.Services;

public sealed class Player : Component
{
    [Property] public PlayerController controller;
    [Property] public ModelRenderer body;
    [Property] public float speed = 10f;

    public bool IsAlive { get; set; } = true;

    private GameObject _corp;

    public void Die(Car killer)
    {
        if (!IsAlive) return;

        controller.ColliderObject.Enabled = false;

        _corp = controller.CreateRagdoll($"Corp: {Connection.Local.DisplayName}");
        var rb = _corp.GetComponentInChildren<Rigidbody>();

        var direction = killer.WorldRotation.Forward;
        rb.Velocity *= direction * killer.Speed * 100f;

        body.Enabled = false;
        //controller.UseCameraControls = false;
        controller.Enabled = false;

        IsAlive = false;
    }

    private void UpdateTeleportToCorp()
    {
        if (IsAlive && !_corp.IsValid()) return;

        WorldPosition = _corp.WorldPosition;
        Log.Info("Die");
    }

    protected override void OnUpdate()
    {
        UpdateTeleportToCorp();
    }
}
