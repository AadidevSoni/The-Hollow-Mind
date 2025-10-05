using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DemonSound : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 20f;
    public float fadeSpeed = 2f;

    public MusicManager musicManager;

    private AudioSource demonAudio;
    private bool wasCompletelySilent = true;

    void Awake()
    {
        demonAudio = GetComponent<AudioSource>();
        demonAudio.loop = true;
        demonAudio.volume = 0f;
        demonAudio.Play();
    }

    void OnEnable()
    {
        wasCompletelySilent = true;
        if (demonAudio != null)
        {
            demonAudio.time = 0f;
            demonAudio.volume = 0f;
        }
    }

    void Update()
    {
        if (player == null || musicManager == null || demonAudio == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        float demonTargetVol = 0f;
        if (distance <= detectionRange)
        {
            float t = 1f - Mathf.Clamp01(distance / detectionRange);
            demonTargetVol = t;

            if (wasCompletelySilent)
            {
                demonAudio.time = 0f;
                demonAudio.Play();
                wasCompletelySilent = false;
            }
        }

        demonAudio.volume = Mathf.Lerp(demonAudio.volume, demonTargetVol, Time.deltaTime * fadeSpeed);

        if (demonAudio.volume <= 0.01f)
        {
            wasCompletelySilent = true;
        }

        AudioSource activeMusic = musicManager.IsInDemonDimension ? musicManager.demonMusic : musicManager.realWorldMusic;
        float targetVolume = distance <= detectionRange ? 0f : 1f;
        activeMusic.volume = Mathf.Lerp(activeMusic.volume, targetVolume, Time.deltaTime * fadeSpeed);
    }
}
