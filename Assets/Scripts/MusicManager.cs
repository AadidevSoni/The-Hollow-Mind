using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioSource realWorldMusic;
    public AudioSource demonMusic;
    public float fadeDuration = 1.5f;

    private Coroutine currentFade;
    [HideInInspector]
    public bool IsInDemonDimension = false;

    void Start()
    {
        realWorldMusic.volume = 1f;
        realWorldMusic.Play();

        demonMusic.volume = 0f;
        demonMusic.Stop();
    }

    public void EnterDemonDimension()
    {
        if (IsInDemonDimension) return;
        IsInDemonDimension = true;

        if (currentFade != null) StopCoroutine(currentFade);
        demonMusic.Play();
        currentFade = StartCoroutine(FadeMusic(realWorldMusic, demonMusic));
    }

    public void ExitDemonDimension()
    {
        if (!IsInDemonDimension) return;
        IsInDemonDimension = false;

        if (currentFade != null) StopCoroutine(currentFade);
        realWorldMusic.Play();
        currentFade = StartCoroutine(FadeMusic(demonMusic, realWorldMusic));
    }

    private IEnumerator FadeMusic(AudioSource from, AudioSource to)
    {
        float timer = 0f;
        float startFrom = from.volume;
        float startTo = to.volume;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            from.volume = Mathf.Lerp(startFrom, 0f, t);
            to.volume = Mathf.Lerp(startTo, 1f, t);

            yield return null;
        }

        from.volume = 0f;
        from.Stop();
        to.volume = 1f;
    }
}
