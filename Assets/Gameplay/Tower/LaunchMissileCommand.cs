using System.Collections;
using UnityEngine;

public class LaunchMissileCommand : Command
{
    private readonly Vector2 _destination;
    private readonly MissileLauncher _launcher;

    public LaunchMissileCommand(Vector2 destination, MissileLauncher launcher)
    {
        _destination = destination;
        _launcher = launcher;
    }

    private bool _isFinished;
    public override bool IsFinished => _isFinished;

    public override void Execute()
    {
        _launcher.StartCoroutine(execute());
        IEnumerator execute()
        {
            yield return rotate(); // Rotate to look at the destination
            _launcher.Get().Fire(_destination, _launcher); // Fire missile 
            yield return knockback(); // Animate knockback
            _isFinished = true;
        }

        IEnumerator rotate()
        {
            Vector2 startDir = _launcher.transform.up;
            Vector2 targetDir = _destination - (Vector2)_launcher.transform.position;
            // Missiles will fly along parabola so the cannon should not look directly at the target
            targetDir.x *= 0.5f;
            targetDir.y *= 1.3f;
            targetDir = targetDir.normalized;
            var time = Vector2.Angle(startDir, targetDir)/ _launcher.rotationSpeed;
            yield return AnimUtils.Animate(f =>
            {
                _launcher.transform.up = Vector2.Lerp(startDir, targetDir, f).normalized;
            }, time);
        }

        IEnumerator knockback()
        {
            var startP = _launcher.transform.position;

            // Go knock
            var knockDir = _launcher.transform.up * -1f;
            var knockDest = _launcher.transform.position + knockDir * _launcher.knockbackDistance;
            yield return AnimUtils.Animate(f =>
            {
                _launcher.transform.position = Vector3.Lerp(startP, knockDest, f);
            }, _launcher.knockbackTime * 0.4f);

            // Go back
            yield return AnimUtils.Animate(f =>
            {
                _launcher.transform.position = Vector3.Lerp(knockDest, startP, f);
            }, _launcher.knockbackTime * 0.6f);
        }
    }
}
