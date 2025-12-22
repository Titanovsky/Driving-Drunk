using Sandbox;

public sealed class TriggerFinish : Component, Component.ITriggerListener
{
    public void OnTriggerEnter(GameObject other)
    {
        var ply = other.GetComponent<Player>();
        if (!ply.IsValid()) return;

        Gameplay.Instance.SendFinishToHost();
    }
}
