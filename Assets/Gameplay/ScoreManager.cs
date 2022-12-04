using System.Collections;
using UnityEngine;

public class ScoreManager : MonoBehaviourSingleton<ScoreManager>
{
    public TextMesh textMesh;
    public LevelManager levelManager;
    [GlobalComponent] private AudioManager sfx;

    public int Points { get; private set; }
    public int NextRebuildAt { get; set; }

    public void Start()
    {
        NextRebuildAt = levelManager.PointsToBonusBuilding;
    }

    public void Add(int points)
    {
        sfx.PlayOneShot("Score", null, points/25f);
        Points += points * levelManager.CurrentLevel;
        textMesh.text = Points.ToString();
    }

    public void Colorize(Color color, float time)
    {
        StartCoroutine(colorize(color, time));
        IEnumerator colorize(Color color, float time)
        {
            var oldColor = textMesh.color;
            yield return AnimUtils.ColorLerp
                (c => textMesh.color = c, oldColor, color, time);
        }
    }
}
