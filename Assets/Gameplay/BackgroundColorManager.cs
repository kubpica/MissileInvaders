using System.Collections;
using UnityEngine;

public class BackgroundColorManager : MonoBehaviour
{
    public Color CurrentColor 
    {
        get => Camera.main.backgroundColor;
        set => Camera.main.backgroundColor = value;
    }

    public void Colorize(Color color, float time)
    {
        StartCoroutine(colorize(color, time));
    }

    private IEnumerator colorize(Color color, float time)
    {
        var oldColor = CurrentColor;
        yield return AnimUtils.ColorLerp
            (c => CurrentColor = c, oldColor, color, time);
    }

    public void GameOver()
    {
        StartCoroutine(gameOver());
        IEnumerator gameOver()
        {
            yield return colorize(Color.yellow, 0.25f);
            yield return colorize(Color.red, 0.25f);
            yield return colorize(Color.yellow, 0.25f);
            yield return colorize(Color.red, 0.25f);
            yield return colorize(Color.black, 1f);
        }
    }

    public void ChangeHue(int change, float time)
    {
        StartCoroutine(AnimUtils.ColorHueChange
            (c => CurrentColor = c, CurrentColor, change, time));
    }
}
