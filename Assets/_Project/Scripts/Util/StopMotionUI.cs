using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class StopMotionUI : MonoBehaviour
{
    public Sprite[] frames;
    public float fps = 10f;
    public bool loop = false;
    public bool destroyOnEnd = true;

    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Play()
    {
        if (this == null || gameObject == null) return;

        StartCoroutine(PlayRoutine());
    }

    public void Reset()
    {
        image.sprite = frames[0];
    }

    IEnumerator PlayRoutine()
    {
        float frameDuration = 1f / fps;
        int currentFrame = 0;

        while (currentFrame < frames.Length)
        {
            image.sprite = frames[currentFrame];
            yield return new WaitForSeconds(frameDuration);
            currentFrame++;

            if (loop && currentFrame >= frames.Length)
            {
                currentFrame = 0;
            }
        }

        if (destroyOnEnd) Destroy(gameObject);
        else Reset();
    }
}
