using System.Collections.Generic;
using UnityEngine;

public class BuildingsManager : MonoBehaviour
{
    public List<Building> GetUndamaged()
    {
        var undamaged = new List<Building>();
        for(int i = 0; i<transform.childCount; i++)
        {
            var b = transform.GetChild(i).GetComponent<Building>();
            if (!b.IsDestroyed)
                undamaged.Add(b);
        }
        return undamaged;
    }

    public List<Building> GetDamaged()
    {
        var damaged = new List<Building>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var b = transform.GetChild(i).GetComponent<Building>();
            if (b.IsDestroyed)
                damaged.Add(b);
        }
        return damaged;
    }

    public bool RebuildOne()
    {
        var damaged = GetDamaged();
        if (damaged.Count > 0)
        {
            damaged[Random.Range(0, damaged.Count)].Rebuild();
            return true;
        }
        return false;
    }
}
