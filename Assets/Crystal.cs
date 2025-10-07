using UnityEngine;

public class Crystal : MonoBehaviour
{
    [Header("Objective on Destroy")]
    public string objectiveText = "OBJECTIVE: Destroy all 7 demon crystals";

    private bool destroyed = false;

    public void BreakCrystal()
    {
        if (destroyed) return;
        destroyed = true;

        // Update the objective
        if (ObjectiveManager.Instance != null)
            ObjectiveManager.Instance.SetObjective(objectiveText);

        // Play any effects, sounds, or destroy object
        Destroy(gameObject);
    }

    // Example: detect player attack
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickaxe")) // adjust to your weapon tag
        {
            BreakCrystal();
        }
    }
}
