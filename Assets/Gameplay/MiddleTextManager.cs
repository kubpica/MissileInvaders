using System.Collections;
using UnityEngine;

public class MiddleTextManager : MonoBehaviour
{
    public TextMesh textMesh;

    public void SetText(string text)
    {
        textMesh.text = text;
    }

    public void Show()
    {
        StartCoroutine(show());
    }

    private IEnumerator show()
    {
        yield return AnimUtils.Animate(f =>
        {
            textMesh.gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, f);
        }, 1f);
    }

    public void Hide()
    {
        StartCoroutine(hide());
    }

    private IEnumerator hide()
    {
        yield return AnimUtils.Animate(f =>
        {
            textMesh.gameObject.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, f);
        }, 1f);
    }

    public IEnumerator ShowAndHide(string text, float time)
    {
        SetText(text);
        yield return show();
        yield return new WaitForSeconds(time);
        yield return hide();
    }
}
