using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissileLauncher : PoolerBase<FriendlyMissile>
{
    public FriendlyMissile missilePrefab;
    public CrosshairSpawner crosshairSpawner;

    public float rotationSpeed;
    public float knockbackDistance = 0.1f;
    public float knockbackTime = 0.5f;
    public float missileSpeed = 10f;

    private Queue<Command> _commands = new Queue<Command>();
    private Command _currentCommand;

    private void Start()
    {
        InitPool(missilePrefab);
    }

    protected override void GetSetup(FriendlyMissile missile)
    {
        base.GetSetup(missile);
        missile.transform.SetParent(transform);
        missile.transform.localPosition = new Vector3(0, 0.83f, 0);
        missile.transform.localScale = Vector3.one;
    }

    private void Update()
    {
        processCommands();

        void processCommands()
        {
            if (_currentCommand != null && !_currentCommand.IsFinished)
                return;

            if (!_commands.Any())
                return;

            _currentCommand = _commands.Dequeue();
            _currentCommand.Execute();
        }
    }

    public void AddCommand(Vector3 destination)
    {
        var crosshair = crosshairSpawner.Get();
        crosshair.transform.position = destination.Z(0);

        var command = new LaunchMissileCommand(destination, this, crosshair);
        _commands.Enqueue(command);
    }
}
