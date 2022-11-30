using UnityEngine;

public class Building : MonoBehaviour
{
    public GameObject fullSpriteObject;

    private bool _isDestroyed;
    public bool IsDestroyed
    {
        get => _isDestroyed;
        set
        {
            _isDestroyed = value;
            fullSpriteObject.SetActive(!value);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Explode()
    {
        // Animate break
        var anim = AnimUtils.Instance;
        anim.CrumbleTexture(fullSpriteObject, 8, false);
        fullSpriteObject.GetComponent<SpriteRenderer>().sortingOrder--;
        anim.BreakTexture(fullSpriteObject, false);
        IsDestroyed = true;

        // Spawn explosion
        ExplosionsManager.Instance.Spawn(transform.position, 1.2f, false);
    }
}
