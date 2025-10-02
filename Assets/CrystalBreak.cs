using UnityEngine;

public class CrystalBreak : MonoBehaviour
{
    [Header("Crystal Settings")]
    public Light pointLight;                // Light to destroy when all broken
    public AudioSource audioSource;
    public AudioClip finalBreakSound;       // Plays only when ALL children break

    private Transform[] childCrystals;
    private int remainingCrystals;

    void Start()
    {
        // Fetch all direct children as crystals
        childCrystals = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            childCrystals[i] = transform.GetChild(i);
        }
        remainingCrystals = childCrystals.Length;
    }

    public void BreakOneCrystal()
    {
        if (remainingCrystals <= 0) return; // No children left, do nothing

        // Find all active children
        var activeChildren = new System.Collections.Generic.List<Transform>();
        foreach (var c in childCrystals)
            if (c.gameObject.activeSelf) activeChildren.Add(c);

        if (activeChildren.Count == 0) return; // Safety check

        // Pick one random active child
        Transform chosen = activeChildren[Random.Range(0, activeChildren.Count)];
        chosen.gameObject.SetActive(false);
        remainingCrystals--;

        // If that was the last child
        if (remainingCrystals == 0)
        {
            // Destroy point light immediately
            if (pointLight != null)
                Destroy(pointLight.gameObject);

            // Play final break sound
            if (audioSource && finalBreakSound)
                audioSource.PlayOneShot(finalBreakSound);

            // Destroy parent crystal
            Destroy(gameObject);
        }
    }
}
