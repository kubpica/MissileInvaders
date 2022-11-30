using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private const float TIME = 1;

    private float _size;

    public void Fire(Vector3 position, float size)
    {
        StartCoroutine(fire());
        IEnumerator fire()
        {
            // Init
            transform.position = position;
            transform.localScale = Vector3.zero;
            
            _size = size;
            var targetSize = Vector3.one * size;

            // Animate resizing & color
            yield return resizeAndColor(0.6f, Color.red, 0.25f);
            yield return resizeAndColor(0.3f, Color.yellow, 0.25f);
            yield return resizeAndColor(1f, Color.red, 0.25f);
            yield return resizeAndColor(0f, Color.yellow, 0.25f);

            // End
            ExplosionsManager.Instance.Despawn(this);

            IEnumerator resizeAndColor(float sizeP, Color color, float timeP)
            {
                timeP *= TIME;
                StartCoroutine(colorize(color, timeP));
                yield return resize(targetSize*sizeP, timeP);
            }
        }

        IEnumerator resize(Vector3 targetSize, float time)
        {
            var startSize = transform.localScale;
            yield return AnimUtils.Animate(f => 
            {
                transform.localScale = Vector3.Lerp(startSize, targetSize, f);
            }, time);
        }

        IEnumerator colorize(Color color, float time)
        {
            var sr = GetComponent<SpriteRenderer>();
            yield return AnimUtils.ColorizeSpriteRenderer(sr, color, time);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Explode enemy missile
        var missile = other.GetComponent<EnemyMissile>();
        if (missile != null)
        {
            missile.Explode(_size * 1.2f);
        }
    }
}
