public class RedEnemyMissileSpawner : PoolerBase<EnemyMissile>
{
    public RedEnemyMissile redMissilePrefab;

    private void Start()
    {
        InitPool(redMissilePrefab);
    }
}
