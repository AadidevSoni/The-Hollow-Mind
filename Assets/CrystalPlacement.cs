using UnityEngine;

public class CrystalPlacement : MonoBehaviour
{
    public Transform[] crystalSlots = new Transform[7];
    private int currentIndex = 0;

    // Place the crystal and return true if placed
    public bool PlaceCrystal(GameObject crystal)
    {
        if (currentIndex >= crystalSlots.Length || crystal == null) return false;

        // Parent to the next empty slot
        crystal.transform.SetParent(crystalSlots[currentIndex]);
        crystal.transform.localPosition = Vector3.zero;
        crystal.transform.localRotation = Quaternion.identity;
        crystal.SetActive(true); // Make sure it's active
        // Optional: add spinning effect
        Rotator rot = crystal.GetComponent<Rotator>();
        if (rot != null) rot.enabled = true;

        currentIndex++;

        if (currentIndex >= crystalSlots.Length)
        {
            Debug.Log("All crystals placed! Event triggered!");
            // TODO: Trigger your special event here
        }

        return true;
    }
}
