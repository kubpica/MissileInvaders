using System.Collections;
using UnityEngine;

public class LaunchMissileCommand : Command
{
    private readonly Vector2 _destination;
    private readonly MissileLauncher _launcher;
    private readonly Crosshair _crosshair;

    public LaunchMissileCommand(Vector2 destination, MissileLauncher launcher, Crosshair crosshair)
    {
        _destination = destination;
        _launcher = launcher;
        _crosshair = crosshair;
    }

    private bool _isFinished;
    public override bool IsFinished => _isFinished;

    private Coroutine _coroutine;
    private bool _isMissileFired;
    private AudioSource _rotateSound;

    public override void Execute()
    {
        var sfx = AudioManager.Instance;
        _coroutine = _launcher.StartCoroutine(execute());
        IEnumerator execute()
        {
            yield return rotate(); // Rotate to look at the destination
            fire(); // Fire missile 
            yield return knockback(); // Animate knockback
            _isFinished = true;
        }

        IEnumerator rotate()
        {
            _rotateSound = sfx.Play("RotateCannon", _launcher.gameObject);
            Vector2 startDir = _launcher.transform.up;
            Vector2 targetDir = _destination - (Vector2)_launcher.transform.position;
            // Missiles will fly along parabola so the cannon should not look directly at the target
            targetDir.x *= 0.5f;
            targetDir.y *= 1.3f;
            targetDir = targetDir.normalized;
            var time = Vector2.Angle(startDir, targetDir)/ _launcher.rotationSpeed;
            _crosshair.Colorize(Color.red, time);
            yield return AnimUtils.Animate(f =>
            {
                _launcher.transform.up = Vector2.Lerp(startDir, targetDir, f).normalized;
            }, time);
            _rotateSound.Stop();
            _rotateSound = null;
        }

        void fire()
        {
            sfx.PlayAtPoint("CannonBlast", _launcher.transform.position);
            _launcher.Get().Fire(_destination, _launcher, _crosshair);
            _isMissileFired = true;
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

    public override void Cancel()
    {
        if (_isFinished || _isMissileFired && _launcher.isActiveAndEnabled)
            return;

        if (_coroutine != null)
        {
            _launcher.StopCoroutine(_coroutine);
        }
        if (!_isMissileFired) 
        {
            _launcher.crosshairSpawner.Release(_crosshair);
        }
        if (_rotateSound != null)
        {
            _rotateSound.Stop();
        }
        _isFinished = true;
    }
}
