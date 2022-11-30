using UnityEngine;

public class ScoreManager : MonoBehaviourSingleton<ScoreManager>
{
    public TextMesh textMesh;
    public LevelManager levelManager;

    public int Points { get; private set; }

    public void Add(int points)
    {
        Points += points * levelManager.CurrentLevel;
        textMesh.text = Points.ToString();
    }
}
