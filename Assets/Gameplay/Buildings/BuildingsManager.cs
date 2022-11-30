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
}
