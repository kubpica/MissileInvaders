using UnityEngine;

public class BuildingCollider : MonoBehaviour
{
    public Building building;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var missile = other.GetComponent<EnemyMissile>();
        if (missile != null)
        {
            missile.Explode(1, false);
            building.Explode();
        }
    }
}
