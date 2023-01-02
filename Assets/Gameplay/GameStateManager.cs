using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviourSingleton<GameStateManager>
{
    public TowerController controller;
    public TextDisplayManager middleText;
    public LevelManager levelManager;
    public BackgroundColorManager background;
    public Ground ground;
    public TowerStorage towerStorage;
    public MissileLauncher launcher;
    public BuildingsManager buildings;
    public ScoreManager score;
    public Credits credits;
    [GlobalComponent] private AudioManager sfx;

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
                case GameState.BeforeGame:
                    StopAllCoroutines();
                    middleText.StopAllCoroutines();
                    credits.StopAllCoroutines();
                    credits.gameObject.SetActive(false);
                    Screen.sleepTimeout = SleepTimeout.NeverSleep;
                    break;
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
                    StartCoroutine(beforeGame());
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
                    Screen.sleepTimeout = SleepTimeout.SystemSetting;
                    sfx.Play("GameOver");
                    background.GameOver();
                    middleText.SetText("THE END");
                    middleText.Show();
                    score.Confirm();
                    break;
            }

            IEnumerator beforeGame()
            {
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
                controller.enabled = false;
                credits.gameObject.SetActive(true);
                credits.Play();
                yield return middleText.ShowAndHide("Missile Invaders v1.1", 1);
                middleText.SetText("Click anywhere to start the game");
                middleText.Show();
            }

            IEnumerator startLevel()
            {
                sfx.Play("StartLevel");
                var lvl = levelManager.CurrentLevel + 1;
                if (lvl > 1)
                {
                    background.ChangeHue(3 * _hueDirection, 3f);
                    ground.ChangeHue(3 * _hueDirection, 3f);
                }
                else
                {
                    score.RefreshText();
                }
                yield return middleText.ShowAndHide($"Day {lvl}", 1);
                CurrentState = GameState.Level;
            }

            IEnumerator endLevel()
            {
                if (launcher.gameObject.activeSelf)
                    sfx.PlayAtPoint("EndLevel", launcher.transform.position);
                yield return new WaitForSeconds(2f);

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

    private int _hueDirection;

    private void Start()
    {
        CurrentState = GameState.BeforeGame;
        levelManager.onLevelEnded.AddListener(() => CurrentState = GameState.LevelEnded);
        _hueDirection = Random.value > 0.5f ? 1 : -1;
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
