using Sandbox;

public sealed class Gameplay : Component
{
    public static Gameplay Instance { get; set; }

    public void Finish()
    {
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
