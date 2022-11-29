using UnityEngine;

public class TowerStorage : MonoBehaviour
{
    public Magazine leftMagazine;
    public Magazine rightMagazine;
    public Magazine cannonMagazine;

    private bool _leftReady = true;
    private Magazine MissileMagazine => _leftReady ? leftMagazine : rightMagazine;

    public bool Reload()
    {
        if (cannonMagazine.Consume())
        {
            leftMagazine.RestoreAll();
            rightMagazine.RestoreAll();
            _leftReady = true;
            return true;
        }
        return false;
    }

    public bool ConsumeMissile()
    {
        if (!MissileMagazine.Consume())
        {
            return false;
        }

        _leftReady = !_leftReady;
        if (MissileMagazine.IsEmpty)
            Reload();

        return true;
    }

    public void ConsumeAll()
    {
        leftMagazine.ConsumeAll();
        rightMagazine.ConsumeAll();
        cannonMagazine.ConsumeAll();
    }

    public void RestoreAll()
    {
        leftMagazine.RestoreAll();
        rightMagazine.RestoreAll();
        cannonMagazine.RestoreAll();
    }
}
