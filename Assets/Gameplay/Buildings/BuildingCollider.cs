using UnityEngine;

public class BuildingCollider : MonoBehaviourExtended
{
    [ParentComponent] private Building building;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var missile = other.GetComponent<EnemyMissile>();
        if (missile != null)
        {
            missile.Explode(1);
            building.Explode();
        }
    }
}
