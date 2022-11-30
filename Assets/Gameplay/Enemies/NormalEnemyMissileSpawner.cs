public class NormalEnemyMissileSpawner : PoolerBase<EnemyMissile>
{
    public NormalEnemyMissile normalMissilePrefab;

    private void Start()
    {
        InitPool(normalMissilePrefab, 20, 40);
    }
}
