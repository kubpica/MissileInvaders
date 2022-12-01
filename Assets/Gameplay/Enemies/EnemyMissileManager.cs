using UnityEngine;

[RequireComponent(typeof(NormalEnemyMissileSpawner))]
[RequireComponent(typeof(RedEnemyMissileSpawner))]
public class EnemyMissileManager : MonoBehaviourSingleton<EnemyMissileManager>
{
    [OwnComponent] private NormalEnemyMissileSpawner normalPool;
    [OwnComponent] private RedEnemyMissileSpawner redPool;

    public int SpawnedCount { get; private set; }

    public EnemyMissile Spawn(float speed)
    {
        if (Random.value < 0.25f)
            return SpawnRed(speed);
        else
            return SpawnNormal(speed);
    }
    public EnemyMissile SpawnNormal(float speed) => spawn(normalPool, speed);
    public EnemyMissile SpawnRed(float speed) => spawn(redPool, speed);
    private EnemyMissile spawn(PoolerBase<EnemyMissile> pool, float speed)
    {
        SpawnedCount++;
        var missile = pool.Get();
        missile.transform.position = new Vector2(Random.Range(-7f, 7f), 10.25f);
        var targetPosition = new Vector2(Random.Range(-7f, 7f), -0.4f);
        missile.Fire(targetPosition, speed);
        return missile;
    }

    public void Despawn(EnemyMissile missile)
    {
        SpawnedCount--;
        if (missile is NormalEnemyMissile)
            despawn(normalPool, missile);
        else
            despawn(redPool, missile);
    }
    private void despawn(PoolerBase<EnemyMissile> pool, EnemyMissile missile)
    {
        pool.Release(missile);
    }
}
