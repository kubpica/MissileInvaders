using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public TowerController controller;
    public MiddleTextManager middleText;
    public LevelManager levelManager;
    public BackgroundColorManager backgroundColor;
    public TowerStorage towerStorage;
    public MissileLauncher launcher;

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
                    break;
            }

            IEnumerator startLevel()
            {
                yield return middleText.ShowAndHide($"Level {levelManager.CurrentLevel+1}", 1);
                CurrentState = GameState.Level;
            }

            IEnumerator endLevel()
            {
                //TODO
                while (towerStorage.ConsumeMissile())
                {
                    yield return new WaitForSeconds(0.15f);
                    //TODO Add points per saved missile
                }
                //TODO Add points per saved building

                //TODO Check if any building saved
                towerStorage.RestoreAll();
                launcher.RestoreCannon();
                CurrentState = GameState.BeforeLevel;
            }
        }
    }

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
