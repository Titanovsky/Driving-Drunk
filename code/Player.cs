using Sandbox;
using Sandbox.Diagnostics;
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
        if (IsProxy) return;

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

    public void Respawn()
    {
        if (IsProxy) return;

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

    private async Task RespawnAsync(float seconds)
    {
        await Task.DelaySeconds(seconds);

        Respawn();
    }

    private void RotateUpdate()
    {
        if (IsProxy) return;

        if (!IsAlive || body is null)
            return;

        // Берём только горизонтальное движение мыши
        var mouseDeltaX = Input.MouseDelta.x;
        var mouseSensitivity = Preferences.Sensitivity * 0.25f;

        Angles input = Input.AnalogLook;
        input *= mouseSensitivity;

        Angles eyeAngles = controller.EyeAngles;
        eyeAngles += input;
        eyeAngles.roll = 0f;
        eyeAngles.pitch = 0f;
        //if (15f > 0f)
        //{
        //    eyeAngles.pitch = eyeAngles.pitch.Clamp(0f - 15f, 15f);
        //}

        controller.EyeAngles = eyeAngles;

        //controller.EyeAngles = new Angles(0f,1000f * -Time.Delta,0f);
        Log.Info(controller.EyeAngles);

        //// Локальные углы тела относительно родителя
        //var angles = controller.Renderer.LocalRotation.Angles();

        //// Крутим только yaw
        //angles.yaw += mouseDeltaX * mouseSensitivity;
        //angles.pitch = 0f;
        //angles.roll = 0f;

        //controller.Renderer.LocalRotation = angles.ToRotation();
    }

    private void TeleportToCorpUpdate()
    {
        if (IsProxy) return;

        if (IsAlive && !_corp.IsValid()) return;

        WorldPosition = _corp.WorldPosition;
    }

    private void InitStart()
    {
        _transformRespawn = WorldTransform;

        HudWorld.Name = Connection.Local.DisplayName;
    }

    protected override void OnStart()
    {
        InitStart();
    }

    protected override void OnUpdate()
    {
        TeleportToCorpUpdate();
        RotateUpdate();
    }
}
