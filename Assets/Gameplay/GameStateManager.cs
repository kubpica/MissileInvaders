using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviourSingleton<GameStateManager>
{
    public TowerController controller;
    public MiddleTextManager middleText;
    public LevelManager levelManager;
    public BackgroundColorManager backgroundColor;
    public TowerStorage towerStorage;
    public MissileLauncher launcher;
    public BuildingsManager buildings;
    public ScoreManager score;

    public enum GameState
    {
        BeforeGame,
        BeforeLevel,
        Level,
        LevelEnded,
        GameOver
    }

    private GameState _currentState;
    public GameState CurrentState 
    {
        get => _currentState;
        private set
        {
            // On state exit
            switch (_currentState)
            {
                case GameState.Level:
                    controller.enabled = false;
                    launcher.CancelCommands();
                    break;
            }

            _currentState = value;

            // On state enter
            switch (value)
            {
                case GameState.BeforeGame:
                    controller.enabled = false;
                    middleText.SetText("Click anywhere to start the game");
                    middleText.Show();
                    break;
                case GameState.BeforeLevel:
                    StartCoroutine(startLevel());
                    break;
                case GameState.Level:
                    controller.enabled = true;
                    levelManager.RunNextLevel();
                    break;
                case GameState.LevelEnded:
                    StartCoroutine(endLevel());
                    break;
                case GameState.GameOver:
                    backgroundColor.GameOver();
                    middleText.SetText("GAME OVER");
                    middleText.Show();
                    break;
            }

            IEnumerator startLevel()
            {
                yield return middleText.ShowAndHide($"Level {levelManager.CurrentLevel+1}", 1);
                CurrentState = GameState.Level;
            }

            IEnumerator endLevel()
            {
                while (towerStorage.ConsumeMissile())
                {
                    // Add points per saved missile
                    score.Add(5);
                    yield return new WaitForSeconds(0.15f);
                }


                // Add points per saved building
                var undamaged = buildings.GetUndamaged();
                foreach (var b in undamaged)
                {
                    b.Hide();
                    score.Add(50);
                    yield return new WaitForSeconds(0.4f);
                }

                // Check if a bonus building is available
                var canRebuild = score.Points > score.NextRebuildAt;
                if (canRebuild)
                {
                    while (score.Points > score.NextRebuildAt)
                        score.NextRebuildAt += levelManager.PointsToBonusBuilding;
                    
                    if(buildings.RebuildOne())
                        yield return new WaitForSeconds(1f);
                }

                // Check if any building survived
                if (undamaged.Count > 0 || canRebuild)
                {
                    towerStorage.RestoreAll();
                    launcher.RestoreCannon();
                    undamaged.ForEach(b => b.Show());
                    CurrentState = GameState.BeforeLevel;
                }
                else
                {
                    CurrentState = GameState.GameOver;
                }
            }
        }
    }

    public bool IsAlive => launcher.gameObject.activeSelf || buildings.IsAnyUndamaged();

    private void Start()
    {
        CurrentState = GameState.BeforeGame;
        levelManager.onLevelEnded.AddListener(() => CurrentState = GameState.LevelEnded);
    }

    private void Update()
    {
        switch (_currentState)
        {
            case GameState.BeforeGame:
                if (Input.GetMouseButtonDown(0))
                {
                    middleText.StopAllCoroutines();
                    CurrentState = GameState.BeforeLevel;
                }
                break;
            case GameState.GameOver:
                if (Input.GetMouseButtonDown(0))
                {
                    SceneManager.LoadScene("Gameplay");
                }
                break;
        }
    }
}
