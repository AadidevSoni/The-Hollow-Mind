using UnityEngine;
using System.Collections.Generic;

public class CrystalManager : MonoBehaviour
{
    [Header("Crystals in Scene")]
    public GameObject[] crystalsArray;

    private List<GameObject> remainingCrystals = new List<GameObject>();

    [Header("Event when all destroyed")]
    public GameObject eventTrigger;

    void Start()
    {
        remainingCrystals = new List<GameObject>(crystalsArray);
    }

    void Update()
    {
        remainingCrystals.RemoveAll(crystal => crystal == null);

        if (remainingCrystals.Count == 0)
        {
            AllCrystalsDestroyed();
        }
    }

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
        if (winTriggered) return;
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
