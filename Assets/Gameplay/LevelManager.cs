using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviourExtended
{
    [GlobalComponent] private EnemyMissileManager missiles;
    [GlobalComponent] private GameStateManager gameState;

    public UnityEvent onLevelEnded;

    public int CurrentLevel { get; private set; }
    public int PointsToBonusBuilding { get; private set; } = 10000;

    private bool _isRunning;
    private int _missilesToSpawn;
    private float _minTimeBetweenStages = 3;
    private float _maxTimeBetweenStages = 10;
    private float _minSpeed = 0.90f;
    private float _maxSpeed = 1.25f;

    public void RunNextLevel()
    {
        if (_isRunning) 
        {
            Debug.LogError("Level is already running.");
            return;
        }

        CurrentLevel++;
        _missilesToSpawn = 8 + 2*CurrentLevel;
        if (CurrentLevel > 1)
        {
            _minTimeBetweenStages = Mathf.Max(0.5f, _minTimeBetweenStages-0.5f);
            _maxTimeBetweenStages = Mathf.Max(3f, _maxTimeBetweenStages-0.5f);
            _minSpeed = Mathf.Min(1.5f, _minSpeed + 0.05f);
            _maxSpeed = Mathf.Min(3f, _maxSpeed + 0.1f);
            if (CurrentLevel % 10 == 1)
            {
                PointsToBonusBuilding += 10000;
            }
        }
        StartCoroutine(run());
    }

    private IEnumerator run()
    {
        _isRunning = true;
        while (_missilesToSpawn > 0 && gameState.IsAlive)
        {
            nextStage();
            var timeToNextStage = Random.Range(_minTimeBetweenStages, _maxTimeBetweenStages);
            yield return waitForSecondsOrStageCleared(timeToNextStage);
        }
        while (missiles.SpawnedCount > 0)
            yield return null;
        onLevelEnded.Invoke();
        _isRunning = false;

        void nextStage()
        {
            int missilesThisStage = Random.Range(1, 7);
            missilesThisStage = Mathf.Min(missilesThisStage, _missilesToSpawn);
            float speedThisStage = Random.Range(_minSpeed, Mathf.Lerp(_minSpeed, _maxSpeed, (7-missilesThisStage)/6f));
            for(int i = 0; i<missilesThisStage; i++)
            {
                missiles.Spawn(speedThisStage);
                _missilesToSpawn--;
            }
        }

        IEnumerator waitForSecondsOrStageCleared(float time)
        {
            float timer = 0;
            while (timer < time)
            {
                // If all enemy missiles destroyed speed up the next stage
                if (missiles.SpawnedCount == 0)
                {
                    var timeLeft = time-timer;
                    yield return new WaitForSeconds(Mathf.Min(1, timeLeft));
                    yield break;
                }

                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}
