using System.Threading.Tasks;
using static Sandbox.Connection;

public sealed class Player : Component, Component.INetworkListener
{
    [Property] public PlayerController controller;
    [Property] public ModelRenderer body;
    [Property] public HudWorld HudWorld;
    [Property] public float speed = 10f;

    [Property] public GameObject BombPrefab { get; set; }
    //[Property] public GameObject PickUpHealPrefab { get; set; }
    [Property] public GameObject ShootObj;
    public PickUpEnum PickUp { get; set; } = PickUpEnum.None;

    [Sync] public bool IsAlive { get; private set; } = true;
    private float _timeRespawn = 2.4f;

    private GameObject _corp;
    private Transform _transformRespawn;
    private float _multipleMouseSens = 0.25f; // from facepunch.playercontroller
    private float _multipleVelocity = 2000f;

    public void TakePickUp(PickUp pickup)
    {
        if (IsProxy) return;
        if (!IsAlive) return;

        PickUp = pickup.TypePickUp;
    }

    private void ActivatePickUp()
    {
        if (!IsAlive) return;
        if (PickUp == PickUpEnum.None) return;

        switch (PickUp)
        {
            default: break;
            case PickUpEnum.Bomb: ActivatePickUpBomb(); break;
        }

        PickUp = PickUpEnum.None;
    }

    private void ActivatePickUpBomb()
    {
        if (!IsAlive) return;
        if (PickUp == PickUpEnum.None) return;

        var obj = BombPrefab.Clone(ShootObj.WorldPosition, rotation: body.WorldRotation);
        obj.NetworkSpawn();
    }

    private void InputActivatePickUp()
    {
        if (Input.Down("Attack1"))
            ActivatePickUp();
    }

    public void Die(Vector3 direction, float speed)
    {
        if (IsProxy) return;

        if (!IsAlive) return;
        IsAlive = false;

        controller.ColliderObject.Enabled = false;

        _corp = controller.CreateRagdoll($"Corp: {Local.DisplayName}");
        _corp.NetworkSpawn();
        var rb = _corp.GetComponentInChildren<Rigidbody>();

        HideBody();

        rb.Velocity = direction * speed;

        controller.Enabled = false;

        _ = RespawnAsync(_timeRespawn);
    }

    public void Die(Car killer)
    {
        if (IsProxy) return;

        if (!IsAlive) return;
        IsAlive = false;

        controller.ColliderObject.Enabled = false;

        _corp = controller.CreateRagdoll($"Corp: {Connection.Local.DisplayName}");
        _corp.NetworkSpawn();
        var rb = _corp.GetComponentInChildren<Rigidbody>();

        HideBody();

        var direction = (WorldPosition - killer.WorldPosition).Normal;
        rb.Velocity = direction * killer.Speed * _multipleVelocity;

        controller.Enabled = false;

        _ = RespawnAsync(_timeRespawn);
    }

    [Rpc.Broadcast]
    public void HideBody()
    {
        body.GameObject.Enabled = false;
        //HudWorld.Enabled = false;
    }

    [Rpc.Broadcast]
    public void ShowBody()
    {
        body.GameObject.Enabled = true;
        //HudWorld.Enabled = true;
    }

    public void Respawn()
    {
        if (IsProxy) return;

        if (_corp.IsValid())
            _corp.Destroy();

        WorldPosition = _transformRespawn.Position;
        WorldRotation = _transformRespawn.Rotation;

        ShowBody();
        controller.ColliderObject.Enabled = true;
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
        var mouseSensitivity = Preferences.Sensitivity * _multipleMouseSens;

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
    }

    private void TeleportToCorpUpdate()
    {
        if (IsProxy) return;

        if (IsAlive && !_corp.IsValid()) return;

        WorldPosition = _corp.WorldPosition;
    }

    private void InitStart()
    {
        if (IsProxy) return;

        HudWorld.Name = Connection.Local.DisplayName;

        _transformRespawn = WorldTransform;

        Scene.Camera.GameObject.Parent = GameObject;
        Scene.Camera.LocalPosition = new Vector3(-105, -293, 282);
    }

    protected override void OnStart()
    {
        InitStart();
    }

    protected override void OnUpdate()
    {
        InputActivatePickUp();
        TeleportToCorpUpdate();
        RotateUpdate();
    }
}
