using System.Collections;
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

    public void Rebuild()
    {
        StartCoroutine(rebuild());
        IEnumerator rebuild()
        {
            IsDestroyed = false;
            var sr = fullSpriteObject.GetComponent<SpriteRenderer>();
            sr.color = Color.white.A(0);
            sr.sortingOrder = 1;
            var score = ScoreManager.Instance;
            score.Colorize(new Color(1, 0.87451f, 0), 0.5f);
            yield return AnimUtils.Animate(f => sr.color = sr.color.A(Mathf.Lerp(0, 1, f)), 0.5f);
            yield return new WaitForSeconds(0.5f);
            score.Colorize(Color.white, 0.5f);
        }
    }
}
