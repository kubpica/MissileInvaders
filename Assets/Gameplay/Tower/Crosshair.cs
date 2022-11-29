using System.Collections;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public void Colorize(Color color, float time)
    {
        StartCoroutine(colorize());
        IEnumerator colorize()
        {
            var sr = GetComponent<SpriteRenderer>();
            Color startColor = sr.color;
            yield return AnimUtils
                .ColorLerp(c => sr.color = c, startColor, color, time);
        }
    }
}
