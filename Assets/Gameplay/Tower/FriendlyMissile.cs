using System.Collections;
using UnityEngine;

public class FriendlyMissile : MonoBehaviour
{
    public void Fire(Vector3 destination, MissileLauncher launcher, Crosshair crosshair)
    {
        StartCoroutine(fire());
        IEnumerator fire()
        {
            // Fly forward
            var forwardFlyDistance = 0.64f;
            var forwardP = transform.position + launcher.transform.up * forwardFlyDistance;
            transform.up = launcher.transform.up;
            yield return AnimUtils.Animate(f =>
            {
                transform.position = Vector3.Lerp(launcher.transform.position, forwardP, f);
            }, forwardFlyDistance / launcher.missileSpeed);

            // Fly along parabola
            transform.parent = null;
            var targetV = destination-transform.position;
            var internalV = forwardP+transform.up - transform.position;
            Debug.DrawLine(launcher.transform.position.Z(100), (forwardP+transform.up).Z(100), Color.red, 2, true);

            var time = (targetV.magnitude+1) / launcher.missileSpeed;
            crosshair.Colorize(Color.clear, time);
            yield return AnimUtils.AnimateAlongParabola(gameObject, internalV, targetV, time);

            // BOOM!
            AnimUtils.Instance.BreakTexture(gameObject, false);
            launcher.Release(this);
            launcher.crosshairSpawner.Release(crosshair);

            //TODO Explosion
        }
    }
}
