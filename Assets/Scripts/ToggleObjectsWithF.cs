using UnityEngine;
using System.Collections;

public class ToggleObjectsWithF : MonoBehaviour
{
    [Header("Objects to Toggle")]
    public GameObject[] objectsToToggle;

    [Header("Player Inventory (Optional)")]
    public PlayerInventory playerInventory;

    [Header("Dimension Switcher Reference")]
    public DimensionSwitcher dimensionSwitcher;

    private bool isVisible = false;

    // NEW: To ensure the objective is updated only once
    private bool objectiveSetForCrystal = false;

    private void OnEnable()
    {
        if (FKeyManager.Instance != null)
            FKeyManager.Instance.OnFKeyPressed += OnFKeyPressed;
        else
            StartCoroutine(WaitForFKeyManager());
    }

    private void OnDisable()
    {
        if (FKeyManager.Instance != null)
            FKeyManager.Instance.OnFKeyPressed -= OnFKeyPressed;
    }

    private IEnumerator WaitForFKeyManager()
    {
        while (FKeyManager.Instance == null)
            yield return null;

        FKeyManager.Instance.OnFKeyPressed += OnFKeyPressed;
    }

    private void Update()
    {
        if (dimensionSwitcher != null && !dimensionSwitcher.IsInDemonDimension)
        {
            foreach (GameObject obj in objectsToToggle)
            {
                if (obj == null)
                    continue;

                if (playerInventory != null && obj == playerInventory.GetEquippedItem())
                    continue;

                obj.SetActive(false);
            }
            isVisible = false;
        }

        // NEW: Check for any destroyed crystal and set objective once
        if (!objectiveSetForCrystal && objectsToToggle != null)
        {
            foreach (GameObject obj in objectsToToggle)
            {
                if (obj == null)
                {
                    // Set objective
                    if (ObjectiveManager.Instance != null)
                    {
                        ObjectiveManager.Instance.SetObjective("OBJECTIVE: Place the crystal's heart in HOLY FIRE");
                        Debug.Log("Objective updated: Place the crystal in HOLY FIRE");
                    }

                    // Equip crystal
                    if (playerInventory != null)
                    {
                        playerInventory.EquipCrystal(); // <-- Call it here
                    }

                    objectiveSetForCrystal = true; // ensure this runs only once
                    break; // no need to check further
                }
            }
        }
    }

    private void OnFKeyPressed()
    {
        if (dimensionSwitcher != null && dimensionSwitcher.IsInDemonDimension)
        {
            isVisible = !isVisible;

            foreach (GameObject obj in objectsToToggle)
            {
                if (obj == null)
                    continue;

                if (playerInventory != null && obj == playerInventory.GetEquippedItem())
                    continue;

                obj.SetActive(isVisible);
            }
        }
    }
}
