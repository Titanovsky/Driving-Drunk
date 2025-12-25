using Sandbox;

public sealed class TriggerFinish : Component, Component.ITriggerListener
{
    public TimeUntil Delay { get; set; } = 2f;

    public void OnTriggerEnter(GameObject other)
    {
        var ply = other.GetComponent<Player>();
        if (!ply.IsValid()) return;
        if (!ply.IsAlive) return;
        if (!Delay) return;

        Delay = 2f;

        Sandbox.Services.Achievements.Unlock("win_map");

        Log.Info($"{ply.Network.Owner.DisplayName} take finish {GameObject}");

        MapManager.Instance.SendFinishToHost();
    }
}
