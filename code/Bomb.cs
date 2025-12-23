using Sandbox;
using Sandbox.UI;
using System;
using System.Threading.Tasks;

public sealed class Bomb : Component
{
	[Property] public float DelayExplode { get; set; } = 2f;
    [Property] public GameObject ExplodeParticlePrefab { get; set; }
    [Property] public SoundEvent SoundExplode { get; set; }

    private float _radius = 128f;
    private float _speed = 10000f;
    private float _forceThrow = 200f;

    private void Explode()
	{
        if (ExplodeParticlePrefab.IsValid())
        {
            var particleObj = ExplodeParticlePrefab.Clone(WorldPosition);
            particleObj.NetworkSpawn();
        }

        if (SoundExplode.IsValid())
            Sound.Play(SoundExplode, WorldPosition);

        var objects = Scene.FindInPhysics(new Sphere(WorldPosition, _radius));
        foreach (GameObject obj in objects)
        {
            if (GameObject == obj) continue;
            if (!obj.Components.TryGet(out Player ply)) continue;

            ply.Die((ply.WorldPosition - WorldPosition).Normal, _speed);
        }

        GameObject.Destroy();
	}

	private async Task ActivateAsync()
	{
		await Task.DelaySeconds(DelayExplode);

        Explode();
	}

	protected override void OnStart()
	{
        var rb = Components.Get<Rigidbody>();
        if (!rb.IsValid()) return;

        rb.Velocity = WorldRotation.Forward * _forceThrow;

		_ = ActivateAsync();
    }
}
