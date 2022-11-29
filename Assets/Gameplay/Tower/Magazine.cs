using UnityEngine;

public class Magazine : MonoBehaviour
{
    private int _available = 2;

    public bool IsEmpty => _available == 0;

    private void Start()
    {
        _available = transform.childCount;
    }

    /// <summary>
    /// Consumes one child in the magazine.
    /// </summary>
    /// <returns> False if there is no child to consume.</returns>
    public bool Consume()
    {
        if (IsEmpty)
            return false;

        _available--;
        transform.GetChild(_available).gameObject.SetActive(false);
        return true;
    }

    public bool ConsumeAll()
    {
        if (IsEmpty)
            return false;

        for (int i = 0; i < _available; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        _available = 0;
        return true;
    }

    public void RestoreAll()
    {
        _available = transform.childCount;
        for(int i = 0; i<_available; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public GameObject GetNextOne()
    {
        if (IsEmpty)
            return null;

        return transform.GetChild(_available - 1).gameObject;
    }
}
