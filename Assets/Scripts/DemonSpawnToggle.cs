using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class DemonSpawnDelayed : MonoBehaviour
{
    public GameObject demon;
    public Transform player;
    public Camera playerCamera;
    public float spawnDelay = 1f;

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
            demon.SetActive(false);
    }

    private void HandleFKeyPress()
    {
        if (demon == null) return;

        if (!demon.activeSelf)
        {
            Vector3 playerPositionAtPress = player.position;

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

        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            demon.transform.position = hit.position;
        }
        else
        {
            demon.transform.position = spawnPosition + Vector3.up;
        }

        demon.SetActive(true);
    }
}
