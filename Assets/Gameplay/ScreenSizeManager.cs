using UnityEngine;

public class ScreenSizeManager : MonoBehaviour
{
    private Vector2 windowSize = new Vector2(0, 0);

    // Update is called once per frame
    void Update()
    {
        if (windowSize.x != Screen.width || windowSize.y != Screen.height)
        {
            windowSize = new Vector2(Screen.width, Screen.height);
            Debug.Log("SS: " + Screen.width + "x" + Screen.height);
            sizeChanged();
        }
    }

    private void sizeChanged()
    {
        // Make sure horizontal area of [-7.2, 7.2] is always visible
        var camera = Camera.main;
        var ratio = Screen.width / (float)Screen.height;
        var horizontalSize = 5.4f * ratio;
        camera.orthographicSize = horizontalSize < 7.2f ? 7.2f / ratio : 5.4f; 
    }
}
