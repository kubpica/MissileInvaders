using UnityEngine;

public abstract class EnemyMissile : MonoBehaviour
{
    public int points;
    public float explosionMultiplier;
    public float speedMultiplier;

    private Vector2 _direction;
    private float _speed;
    private bool _isFired;
    private bool _isExploding;

    public void Fire(Vector2 targetPosition, float speed)
    {
        _direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.up = _direction;

        _speed = speed * speedMultiplier;
        _isFired = true;
        _isExploding = false;
    }

    public void Explode(float explosionSize, bool canScore)
    {
        if (_isExploding)
            return;
        _isExploding = true;

        // Animate break
        var anim = AnimUtils.Instance;
        anim.CrumbleTexture(gameObject, 4, false);
        anim.BreakTexture(gameObject, false);

        // Spawn explosion
        ExplosionsManager.Instance.Spawn(transform.position, explosionSize * explosionMultiplier, canScore);

        // Score points
        if (canScore)
            ScoreManager.Instance.Add(points);

        // Release to the pool
        _isFired = false;
        EnemyMissileManager.Instance.Despawn(this);
    }

    private void Update()
    {
        if (!_isFired)
            return;

        Vector2 p = transform.position;
        p += _direction * _speed * Time.deltaTime;
        transform.position = p;

        // Hit gound
        if (p.y < -0.3f)
            Explode(0.5f, false);
    }
}
