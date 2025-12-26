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

        switch (MapManager.Instance.MapIndex)
        {
            default:
                break;
            case 1:
                Sandbox.Services.Achievements.Unlock("win_desert");
                break;
            case 2:
                Sandbox.Services.Achievements.Unlock("win_forest");
                break;
            case 3:
                Sandbox.Services.Achievements.Unlock("win_lava");
                break;
        }

        Log.Info($"{ply.Network.Owner.DisplayName} take finish {GameObject}");

        MapManager.Instance.SendFinishToHost();
    }
}
