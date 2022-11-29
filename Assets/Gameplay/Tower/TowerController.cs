using UnityEngine;

public class TowerController : MonoBehaviourExtended
{
    [Component] private MissileLauncher launcher;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Click");
            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            launcher.AddCommand(p);
        }
    }
}
