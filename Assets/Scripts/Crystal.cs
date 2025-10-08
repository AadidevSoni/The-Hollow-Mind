using UnityEngine;

public class Crystal : MonoBehaviour
{
    [Header("Objective on Destroy")]
    private bool destroyed = false;

    public void BreakCrystal()
    {
        if (destroyed) return;
        destroyed = true;


        Destroy(gameObject, 0.2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickaxe"))
        {
            BreakCrystal();
            GetComponent<Collider>().enabled = false;
        }
    }
}
