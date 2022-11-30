using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public void Colorize(Color color, float time)
    {
        var sr = GetComponent<SpriteRenderer>();
        StartCoroutine(AnimUtils.ColorizeSpriteRenderer(sr, color, time));
    }
}
