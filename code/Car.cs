using Sandbox;

public sealed class Car : Component, Component.ITriggerListener, IObjectPool
{
	[Property] public float Speed { get; set; } = 1f;
	[Property] public float DelayToDie { get; set; } = 5f;

    public Vector3 Direction { get; set; } = Vector3.Zero;

    private TimeUntil _timerToDie { get; set; }

    public void Init()
    {
        if (_timerToDie)
            _timerToDie = DelayToDie;
    }

    public void Release()
    {
        GameObject.Enabled = false;
    }

    public void OnTriggerEnter(GameObject other)
    {
        var ply = other.GetComponent<Player>();
        if (!ply.IsValid()) return;

        ply.Die(this);
    }

    private void Moving()
	{
        if (_timerToDie) return;

        var direction = Direction;

		WorldPosition += direction * Speed;
	}

	private void CheckDeath()
	{
		if (!_timerToDie) return;

        Release();
    }

    protected override void OnStart()
    {
        Init();
    }

    protected override void OnUpdate()
    {
        Moving();
        CheckDeath();
    }
}
