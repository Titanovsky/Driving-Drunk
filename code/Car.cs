using Sandbox;

public sealed class Car : Component, Component.ITriggerListener
{
	[Property] public float Speed { get; set; } = 1f;
	[Property] public float DelayToDie { get; set; } = 5f;

    private TimeUntil _timerToDie { get; set; }

    protected override void OnStart()
    {
		_timerToDie = DelayToDie;
    }


    protected override void OnUpdate()
	{
		Moving();
		CheckDeath();
    }

	private void Moving()
	{
		var direction = WorldRotation.Forward;

		WorldPosition += direction * Speed;
	}

	private void CheckDeath()
	{
		if (!_timerToDie) return;

		DestroyGameObject();
	}

    public void OnTriggerEnter(GameObject other)
	{
		var ply = other.GetComponent<Player>();
		if (!ply.IsValid()) return;

		ply.Die(this);
	}
}
