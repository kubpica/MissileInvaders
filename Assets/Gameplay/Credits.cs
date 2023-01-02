using System.Collections;
using UnityEngine;

public class Credits : MonoBehaviour
{
    public TextDisplayManager text;
    public string[] credits;

    public void Play()
    {
        StartCoroutine(play());
        IEnumerator play()
        {
            int i = 0;
            while (true)
            {
                string s = credits[i];
                yield return text.ShowAndHide(s, 1);
                i++;
                i %= credits.Length;
            }
        }
    }
}
