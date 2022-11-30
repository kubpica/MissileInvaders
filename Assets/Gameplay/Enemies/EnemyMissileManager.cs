using UnityEngine;

[RequireComponent(typeof(NormalEnemyMissileSpawner))]
[RequireComponent(typeof(RedEnemyMissileSpawner))]
public class EnemyMissileManager : MonoBehaviourSingleton<EnemyMissileManager>
{
    [OwnComponent] private NormalEnemyMissileSpawner normalPool;
    [OwnComponent] private RedEnemyMissileSpawner redPool;

    public EnemyMissile Spawn()
    {
        if (Random.value < 0.25f)
            return SpawnRed();
        else
            return SpawnNormal();
    }
    public EnemyMissile SpawnNormal() => spawn(normalPool);
    public EnemyMissile SpawnRed() => spawn(redPool);
    private EnemyMissile spawn(PoolerBase<EnemyMissile> pool)
    {
        var missile = pool.Get();
        missile.transform.position = new Vector2(Random.Range(-7f, 7f), 10.25f);
        var targetPosition = new Vector2(Random.Range(-7f, 7f), -0.4f);
        missile.Fire(targetPosition, 1f);
        return missile;
    }

    public void Despawn(EnemyMissile missile)
    {
        if (missile is NormalEnemyMissile)
            despawn(normalPool, missile);
        else
            despawn(redPool, missile);
    }
    private void despawn(PoolerBase<EnemyMissile> pool, EnemyMissile missile)
    {
        pool.Release(missile);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            for (int i = 0; i <= 4; i++) 
            {
                Spawn();
            }
        }
    }
}
