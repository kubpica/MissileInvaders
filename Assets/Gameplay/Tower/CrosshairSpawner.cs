using UnityEngine;

public class CrosshairSpawner : PoolerBase<Crosshair>
{
    public Crosshair crosshairPrefab;

    private void Start()
    {
        InitPool(crosshairPrefab);
    }

    protected override void GetSetup(Crosshair crosshair)
    {
        base.GetSetup(crosshair);
        crosshair.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
