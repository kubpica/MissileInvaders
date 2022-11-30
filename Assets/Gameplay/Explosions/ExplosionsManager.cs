using UnityEngine;

[RequireComponent(typeof(ExplosionSpawner))]
public class ExplosionsManager : MonoBehaviourSingleton<ExplosionsManager>
{
    [OwnComponent] private ExplosionSpawner pool;

    public Explosion Spawn(Vector3 position, float size)
    {
        var explosion = pool.Get();
        explosion.Fire(position, size);
        return explosion;
    }

    public void Despawn(Explosion explosion)
    {
        pool.Release(explosion);
    }
}
