using System.Collections;
using UnityEngine;

public class BackgroundColorManager : MonoBehaviour
{
    public void Colorize(Color color, float time)
    {
        StartCoroutine(colorize(color, time));
    }

    private IEnumerator colorize(Color color, float time)
    {
        var camera = Camera.main;
        var oldColor = camera.backgroundColor;
        yield return AnimUtils.ColorLerp
            (c => camera.backgroundColor = c, oldColor, color, time);
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
}
