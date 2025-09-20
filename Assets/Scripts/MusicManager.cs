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
        demonMusic.volume = 0f;

        realWorldMusic.Play();
        demonMusic.Play();
    }

    public void EnterDemonDimension()
    {
        IsInDemonDimension = true;

        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeMusicAndRestart(realWorldMusic, demonMusic));
    }

    public void ExitDemonDimension()
    {
        IsInDemonDimension = false;

        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeMusicAndRestart(demonMusic, realWorldMusic));
    }

    private IEnumerator FadeMusicAndRestart(AudioSource from, AudioSource to)
    {
        float timer = 0f;
        float startFrom = from.volume;
        float startTo = to.volume;

        // Fade out 'from' and fade in 'to'
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            from.volume = Mathf.Lerp(startFrom, 0f, t);
            to.volume = Mathf.Lerp(startTo, 1f, t);

            yield return null;
        }

        // Ensure final volumes
        from.volume = 0f;
        to.volume = 1f;

        // Restart only the faded-out music
        from.time = 0f;
    }

}
