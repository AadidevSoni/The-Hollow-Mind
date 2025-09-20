using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DemonSound : MonoBehaviour
{
    public Transform player;               // Assign Player
    public float detectionRange = 20f;     // Max distance for demon proximity effect
    public float fadeSpeed = 2f;           // How fast volume adjusts

    public MusicManager musicManager;      // Assign MusicManager in Inspector

    private AudioSource demonAudio;
    private bool wasCompletelySilent = true;

    void Start()
    {
        demonAudio = GetComponent<AudioSource>();
        demonAudio.loop = true;
        demonAudio.volume = 0f;
        demonAudio.Play(); // Always play, adjust volume dynamically
    }

    void Update()
    {
        if (player == null || musicManager == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Demon growl volume based on distance
        float demonTargetVol = 0f;
        if (distance <= detectionRange)
        {
            float t = 1f - Mathf.Clamp01(distance / detectionRange); // Closer = louder
            demonTargetVol = t;

            // If previously completely silent, restart from beginning
            if (wasCompletelySilent)
            {
                demonAudio.time = 0f;
                demonAudio.Play();
                wasCompletelySilent = false;
            }
        }

        demonAudio.volume = Mathf.Lerp(demonAudio.volume, demonTargetVol, Time.deltaTime * fadeSpeed);

        // Track if demonAudio has faded out completely
        if (demonAudio.volume <= 0.01f)
        {
            wasCompletelySilent = true;
        }

        // Fade currently active background music based on proximity
        AudioSource activeMusic = musicManager.IsInDemonDimension ? musicManager.demonMusic : musicManager.realWorldMusic;
        float targetVolume = distance <= detectionRange ? 0f : 1f; // 0 if close, 1 if far
        activeMusic.volume = Mathf.Lerp(activeMusic.volume, targetVolume, Time.deltaTime * fadeSpeed);
    }
}
