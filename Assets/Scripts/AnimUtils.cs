using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimUtils : MonoBehaviourSingleton<AnimUtils>
{
    public static IEnumerator Animate(Action<float> action, float time)
    {
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            if (t > time)
                t = time;

            action(t / time);
            yield return null;
        }
    }

    /// <summary>
    /// Moves the object to the specified point along the parabola.
    /// </summary>
    /// <param name="go"> The object to move.</param>
    /// <param name="internalPoint"> The point of the parabola between the object and the end point. (offset from current position)</param>
    /// <param name="endPoint"> The point at which the object will be stopped. (offset from current position)</param>
    public static IEnumerator AnimateAlongParabola(GameObject go, Vector2 internalPoint, Vector2 endPoint, float time = 1)
    {
        Vector3 basePos = go.transform.position;
        float x1 = 0;
        float y1 = 0;
        float x2 = internalPoint.x;
        float y2 = internalPoint.y;
        float x3 = endPoint.x;
        float y3 = endPoint.y;
        if (Mathf.Abs(x2 - x1) < 0.01f)
        {
            x2 = 0.01f * Mathf.Sign(x2);
        }
        if (Mathf.Abs(x3 - x2) < 0.01f)
        {
            x2 += 0.01f * Mathf.Sign(x2);
            x3 += 0.02f * Mathf.Sign(x3);
        }   
        if (x3 == x1)
            x3 = x2 + 0.021f;
        if (x2 == x1)
            x2 = x1 + 0.01f;

        float denom = (x1 - x2) * (x1 - x3) * (x2 - x3);
        float A = (x3 * (y2 - y1) + x2 * (y1 - y3) + x1 * (y3 - y2)) / denom;
        float B = (x3 * x3 * (y1 - y2) + x2 * x2 * (y3 - y1) + x1 * x1 * (y2 - y3)) / denom;
        float C = (x2 * x3 * (x2 - x3) * y1 + x3 * x1 * (x3 - x1) * y2 + x1 * x2 * (x1 - x2) * y3) / denom;

        float distance = x3 - x1;

        for (float passed = 0.0f; passed < time;)
        {
            passed += Time.deltaTime;
            float f = passed / time;
            if (f > 1) f = 1;

            float x = distance * f;
            float y = A * x * x + B * x + C;
            if (go == null) yield break;

            var p = go.transform.position;
            p.x = basePos.x + x;
            p.y = basePos.y + y;
            p.z = 0;
            
            lookAt2D(p);
            go.transform.position = p;

            yield return 0;
        }

        void lookAt2D(Vector2 position)
        {
            Vector2 v = position - (Vector2)go.transform.position;
            go.transform.up = v.normalized;
        }
    }

    public static IEnumerator ColorLerp(Action<Color> action, Color oldColor, Color newColor, float time)
    {
        yield return AnimUtils.Animate(t => action(Color.Lerp(oldColor, newColor, t)), time);
    }

    public static IEnumerator ColorHueChange(Action<Color> action, Color color, int change, float time)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        float oldHue = h;
        float newHue = h + change / 360f;
        yield return AnimUtils.Animate
            (f =>
            {
                var h = Mathf.Lerp(oldHue, newHue, f) % 1f;
                action(Color.HSVToRGB(h, s, v));
            }, time);
    }

    public static IEnumerator ColorizeSpriteRenderer(SpriteRenderer sr, Color newColor, float time)
    {
        Color startColor = sr.color;
        yield return ColorLerp(c => sr.color = c, startColor, newColor, time);
    }

    /// <summary>
    /// Crumbles passed sprite.
    /// </summary>
    /// <param name="toCrumble"> GameObject containing the sprite to crumble.</param>
    /// <param name="fragmentsAmount"> How many crumbs.</param>
    /// <param name="breakOff"> If false, just animation; otherwise the texture is crumbled for real.</param>
    public void CrumbleTexture(GameObject toCrumble, int fragmentsAmount, bool breakOff)
    {
        var fragments = getCrumbs(toCrumble, fragmentsAmount, breakOff);
        StartCoroutine(AnimateShards(fragments)); //start animation
    }

    private GameObject[] getCrumbs(GameObject textureGO, int fragmentsAmount, bool breakOff)
    {
        GameObject[] fragment = new GameObject[fragmentsAmount];
        Texture2D texture = textureGO.GetComponent<SpriteRenderer>().sprite.texture;
        // Breaking off random blocks from the block
        for (int j = 0; j < fragmentsAmount; j++)
        { 
            float width = Mathf.Floor(texture.width * 0.225f);
            float height = Mathf.Floor(texture.height * 0.225f);

            int randx = Random.Range((int)(width / -2) + 1, (int)(texture.width - 1));
            if (randx < 0)
            {
                width += randx * 2;
                randx = 0;
            }
            else width = Mathf.Min(texture.width - randx, width);

            int randy = Random.Range((int)(height / -2) + 1, (int)(texture.height - 1));
            if (randy < 0)
            {
                height += randy * 2;
                randy = 0;
            }
            else height = Mathf.Min(texture.height - randy, height);

            fragment[j] = CropSprite(textureGO, new Rect(randx, randy, width, height), breakOff, 0.5f);
        }
        return fragment;
    }

    /// <summary>
    /// Crops texture (of passed gameObject) in given place. It can break off given fragment out of the texture or just return a copy.
    /// </summary>
    /// <param name="spriteToCrop"> GameObject containing sprite with texture to crop.</param>
    /// <param name="croppedSpriteRect"> Rectangular section of the texture to crop.</param>
    /// <param name="breakOff"> Do you want to edit passed texture (do a hole in it) or just get copy of its part without changing the original one?</param>
    /// <param name="tornMultiplier"> Multiplier of the hole done in original texture. (The hole can have different size than returned part of texture.)</param>
    /// <returns>
    /// Returns new GameObject containing sprite with cropped part of the texture.
    /// </returns>
    public static GameObject CropSprite(GameObject spriteToCrop, Rect croppedSpriteRect, bool breakOff = true, float tornMultiplier = 1.0f)
    {
        SpriteRenderer rend = spriteToCrop.GetComponent<SpriteRenderer>();
        Sprite s = rend.sprite;
        Texture2D spriteTexture = s.texture;

        if (breakOff)
        {
            // Duplicate the original texture and assign to the material
            Texture2D texture = Instantiate(spriteTexture) as Texture2D;

            Color fillColor = Color.clear;
            int width = (int)(croppedSpriteRect.width * tornMultiplier);
            int height = (int)(croppedSpriteRect.height * tornMultiplier);
            Color[] fillPixels = new Color[(int)(width * height)];

            for (int i = 0; i < fillPixels.Length; i++)
                fillPixels[i] = fillColor;

            if (tornMultiplier != 1.0f)
            {
                int stX = (int)Mathf.Clamp(croppedSpriteRect.x + (croppedSpriteRect.width - width) / 2, 0, spriteTexture.width - 1);
                int stY = (int)Mathf.Clamp(croppedSpriteRect.y + (croppedSpriteRect.height - height) / 2, 0, spriteTexture.height - 1);
                texture.SetPixels(stX, stY, width, height, fillPixels);
            }
            else texture.SetPixels((int)croppedSpriteRect.x, (int)croppedSpriteRect.y, width, height, fillPixels);
            texture.Apply(true);

            // Assigning a new texture (with holes) to the sprite (renderer)
            var pivot = new Vector2()
            {
                x = s.pivot.x / texture.width,
                y = s.pivot.y / texture.height
            };
            rend.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot, s.pixelsPerUnit);
        }

        GameObject croppedSpriteObj = new GameObject("CroppedSprite");
        Sprite croppedSprite = Sprite.Create(spriteTexture, croppedSpriteRect, new Vector2(0, 1), s.pixelsPerUnit);
        croppedSpriteObj.AddComponent<SpriteRenderer>();
        SpriteRenderer cropSpriteRenderer = croppedSpriteObj.GetComponent<SpriteRenderer>();
        cropSpriteRenderer.sprite = croppedSprite;
        cropSpriteRenderer.sortingOrder = rend.sortingOrder + 1;

        croppedSpriteObj.transform.localScale = spriteToCrop.transform.lossyScale;

        var ppuX = s.pixelsPerUnit / spriteToCrop.transform.lossyScale.x;
        var ppuY = s.pixelsPerUnit / spriteToCrop.transform.lossyScale.y;
        croppedSpriteObj.transform.position = Change.X(croppedSpriteObj.transform.position, spriteToCrop.transform.transform.position.x - rend.sprite.pivot.x / ppuX + croppedSpriteRect.x / ppuX); //-croppedSpriteRect.width*0.01f/2;
        croppedSpriteObj.transform.position = Change.Y(croppedSpriteObj.transform.position, spriteToCrop.transform.transform.position.y - rend.sprite.pivot.y / ppuY + croppedSpriteRect.y / ppuY + croppedSpriteRect.height / ppuY);

        var rotation = spriteToCrop.transform.rotation;
        var defaultRotation = new Quaternion(0, 0, 0, 1);
        if (!rotation.Equals(defaultRotation))
        {
            spriteToCrop.transform.rotation = defaultRotation;
            croppedSpriteObj.transform.SetParent(spriteToCrop.transform, true);
            spriteToCrop.transform.rotation = rotation;
            croppedSpriteObj.transform.SetParent(null, true);
        }

        return croppedSpriteObj;
    }

    /// <summary>
    /// Destroys passed gameobject with animation of breaking its texture into 4 pieces.
    /// </summary>
    /// <param name="textureGO"> To break - must contain SpriteRenderer.</param>
    public void BreakTexture(GameObject textureGO, bool destroy = true)
    {
        animateBreak(textureGO);
        if (destroy)
            Destroy(textureGO);
    }

    private void animateBreak(GameObject textureGO, bool bump = false)
    {
        Texture2D texture = textureGO.GetComponent<SpriteRenderer>().sprite.texture;
        GameObject[] fragments = new GameObject[4];
        fragments[0] = CropSprite(textureGO, new Rect(0, texture.height / 2.0f, texture.width / 2.0f, texture.height / 2.0f), false);
        fragments[1] = CropSprite(textureGO, new Rect(texture.width / 2.0f, texture.height / 2.0f, texture.width / 2.0f, texture.height / 2.0f), false);
        fragments[2] = CropSprite(textureGO, new Rect(0, 0, texture.width / 2.0f, texture.height / 2.0f), false);
        fragments[3] = CropSprite(textureGO, new Rect(texture.width / 2.0f, 0, texture.width / 2.0f, texture.height / 2.0f), false);

        if (bump)
        {
            fragments[2].transform.position += new Vector3(-0.10f, 0.10f, 0);
            fragments[3].transform.position += new Vector3(0.10f, 0.10f, 0);
        }

        StartCoroutine(AnimateShards(fragments));
    }

    /// <summary>
    /// Makes passed gameObjects to follow random parabola (as if they where tossed up) and to gradually disappear. Destroys them at the end of animation.
    /// </summary>
    /// <param name="fragments"> Array of gameObjects to animate and destroy.</param>
    /// <param name="time"> The duration of the animation.</param>
    /// <remarks>
    /// Call the function this way:
    /// <c>StartCoroutine (AnimateShards(fragments));</c>
    /// </remarks>
    public static IEnumerator AnimateShards(GameObject[] fragments, float time = 1.0f)
    { //crumble, random parabola animation
        int size = fragments.Length;
        Color c = fragments[0].GetComponent<Renderer>().material.color;
        Vector3[] framentPos = new Vector3[size];
        float[] a = new float[size];
        float[] b = new float[size];
        bool[] toTheRight = new bool[size];
        for (int i = 0; i < size; i++)
        {
            framentPos[i] = fragments[i].transform.position;
            a[i] = Random.Range(-7, -3);
            b[i] = Random.Range(2.0f, 3.5f);
            toTheRight[i] = (Random.value < 0.5f);
        }
        float x;
        for (float passed = 0.0f; passed < time;)
        {
            passed += Time.deltaTime;
            float f = passed / time;
            if (f > 1) f = 1;
            c.a = 1 - f;

            for (int i = 0; i < size; i++)
            {
                if (toTheRight[i])
                {
                    x = 0.8f * f;
                    fragments[i].transform.position = Change.X(fragments[i].transform.position, framentPos[i].x + x);
                    fragments[i].transform.position = Change.Y(fragments[i].transform.position, framentPos[i].y + (a[i] * x * x + b[i] * x - 0.2f));
                }
                else
                {
                    x = -0.8f * f;
                    fragments[i].transform.position = Change.X(fragments[i].transform.position, framentPos[i].x + x);
                    fragments[i].transform.position = Change.Y(fragments[i].transform.position, framentPos[i].y + (a[i] * x * x + b[i] * (-x) - 0.2f));
                }
                fragments[i].GetComponent<Renderer>().material.color = c;
            }

            yield return 0;
        }

        for (int i = 0; i < size; i++)
        {
            Destroy(fragments[i]);
        }
    }
}
