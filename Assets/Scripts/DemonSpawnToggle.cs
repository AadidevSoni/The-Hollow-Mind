using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class DemonSpawnDelayed : MonoBehaviour
{
    public GameObject demon;       // Demon GameObject
    public Transform player;       // Player Transform
    public Camera playerCamera;    // Main Camera
    public float spawnDelay = 1f;  // Delay in seconds before spawning

    private void OnEnable()
    {
        if (FKeyManager.Instance != null)
            FKeyManager.Instance.OnFKeyPressed += HandleFKeyPress;
    }

    private void OnDisable()
    {
        if (FKeyManager.Instance != null)
            FKeyManager.Instance.OnFKeyPressed -= HandleFKeyPress;
    }

    private void Start()
    {
        if (demon != null)
            demon.SetActive(false); // start disabled
    }

    private void HandleFKeyPress()
    {
        if (demon == null) return;

        if (!demon.activeSelf)
        {
            // Capture the player's current position
            Vector3 playerPositionAtPress = player.position;

            // Start coroutine to spawn demon after delay
            StartCoroutine(SpawnDemonAfterDelay(playerPositionAtPress));
        }
        else
        {
            demon.SetActive(false);
        }
    }

    private IEnumerator SpawnDemonAfterDelay(Vector3 spawnPosition)
    {
        yield return new WaitForSeconds(spawnDelay);

        // Ensure demon is on NavMesh near the captured position
        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            demon.transform.position = hit.position;
        }
        else
        {
            // If NavMesh not found, fallback slightly above the position
            demon.transform.position = spawnPosition + Vector3.up;
        }

        demon.SetActive(true);
    }
}
