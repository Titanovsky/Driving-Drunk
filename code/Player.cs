using Sandbox;

public sealed class Player : Component
{
	[Property] public float speed = 10f;

    public void Die()
    {
        Game.Close();
    }
}
