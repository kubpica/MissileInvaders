using UnityEngine;

public class TowerController : MonoBehaviourExtended
{
    [Component] private MissileLauncher launcher;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.y = Mathf.Max(p.y, 1.48f);
            launcher.AddCommand(p);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            launcher.DestroyCannon();
        }
    }
}
