using Sandbox;

public sealed class PickUp : Component, Component.ITriggerListener
{
    [Property] public PickUpEnum TypePickUp { get; private set; } = PickUpEnum.None;

    private void Pickup(Player ply)
    {
        ply.TakePickUp(this);

        GameObject.Destroy();
    }

    public void OnTriggerEnter(GameObject other)
    {
        var ply = other.GetComponent<Player>();
        if (!ply.IsValid()) return;

        Pickup(ply);
    }

    protected override void OnStart()
    {
        
    }

    protected override void OnUpdate()
	{
		WorldRotation *= Rotation.FromYaw(90f * Time.Delta * 2f);
	}
}

public enum PickUpEnum
{
    None,
    Bomb,
    Heal,
    Stone
}