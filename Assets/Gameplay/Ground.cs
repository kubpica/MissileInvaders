using UnityEngine;

public class Ground : MonoBehaviour
{
    private SpriteRenderer[] _renderers;

    public Color CurrentColor 
    {
        get => _renderers[0].color;
        set 
        {
            foreach (var sr in _renderers)
            {
                sr.color = value;
            }
        }
    }

    private void Start()
    {
        _renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void ChangeHue(int change, float time)
    {
        StartCoroutine(AnimUtils.ColorHueChange
            (c => CurrentColor = c, CurrentColor, change, time));
    }
}
