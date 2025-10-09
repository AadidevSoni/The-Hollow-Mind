using UnityEngine;
using System.Collections.Generic;

public class CrystalManager : MonoBehaviour
{
    [Header("Crystals in Scene")]
    public GameObject[] crystalsArray; // Assign 7 crystal GameObjects in Inspector

    private List<GameObject> remainingCrystals = new List<GameObject>();

    [Header("Event when all destroyed")]
    public GameObject eventTrigger; // e.g., canvas with win text

    void Start()
    {
        remainingCrystals = new List<GameObject>(crystalsArray);
    }

    void Update()
    {
        // Clean up destroyed crystals automatically
        remainingCrystals.RemoveAll(crystal => crystal == null);

        // Trigger win event if none remain
        if (remainingCrystals.Count == 0)
        {
            AllCrystalsDestroyed();
        }
    }

    // Optional: call this from individual crystal scripts when destroyed
    public void NotifyCrystalDestroyed(GameObject crystal)
    {
        if (remainingCrystals.Contains(crystal))
        {
            remainingCrystals.Remove(crystal);
        }

        if (remainingCrystals.Count == 0)
        {
            AllCrystalsDestroyed();
        }
    }

    private bool winTriggered = false;
    private void AllCrystalsDestroyed()
    {
        if (winTriggered) return; // Ensure it only triggers once
        winTriggered = true;

        Debug.Log("All crystals destroyed!");

        if (eventTrigger != null)
        {
            WinController win = eventTrigger.GetComponent<WinController>();
            if (win != null)
            {
                win.TriggerWin();
            }
        }
    }
}
