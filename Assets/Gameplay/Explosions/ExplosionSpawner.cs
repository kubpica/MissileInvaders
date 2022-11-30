public class ExplosionSpawner : PoolerBase<Explosion>
{
    public Explosion explosionPrefab;

    private void Start()
    {
        base.InitPool(explosionPrefab);
    }
}
