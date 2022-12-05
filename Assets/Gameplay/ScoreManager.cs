using System.Collections;
using UnityEngine;

public class ScoreManager : MonoBehaviourSingleton<ScoreManager>
{
    public TextMesh textMesh;
    public LevelManager levelManager;
    [GlobalComponent] private AudioManager sfx;

    public int Points { get; private set; }
    public int NextRebuildAt { get; set; }
    public int HiScore 
    { 
        get => PlayerPrefs.GetInt("HiScore", 0);
        set
        {
            PlayerPrefs.SetInt("HiScore", value);
        }
    }

    public void Start()
    {
        NextRebuildAt = levelManager.PointsToBonusBuilding;
        textMesh.text = $"HiScore: {HiScore}";
    }

    public void Add(int points)
    {
        sfx.PlayOneShot("Score", null, points/25f);
        Points += points * levelManager.CurrentLevel;
        RefreshText();
    }

    public void RefreshText()
    {
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

    public void Confirm()
    {
        if (Points > HiScore)
        {
            HiScore = Points;
        }
    }
}
