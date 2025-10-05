using UnityEngine;

public class CrystalBreak : MonoBehaviour
{
    [Header("Crystal Settings")]
    public Light pointLight;
    public AudioSource audioSource;
    public AudioClip finalBreakSound;

    private Transform[] childCrystals;
    private int remainingCrystals;

    void Start()
    {
        childCrystals = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            childCrystals[i] = transform.GetChild(i);
        }
        remainingCrystals = childCrystals.Length;
    }

    public void BreakOneCrystal()
    {
        if (remainingCrystals <= 0) return;

        var activeChildren = new System.Collections.Generic.List<Transform>();
        foreach (var c in childCrystals)
            if (c.gameObject.activeSelf) activeChildren.Add(c);

        if (activeChildren.Count == 0) return;

        Transform chosen = activeChildren[Random.Range(0, activeChildren.Count)];
        chosen.gameObject.SetActive(false);
        remainingCrystals--;

        if (remainingCrystals == 0)
        {
            if (pointLight != null)
                Destroy(pointLight.gameObject);

            if (audioSource && finalBreakSound)
                audioSource.PlayOneShot(finalBreakSound);

            Destroy(gameObject);
        }
    }
}
