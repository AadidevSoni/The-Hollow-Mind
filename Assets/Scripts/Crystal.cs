using UnityEngine;

public class Crystal : MonoBehaviour
{
    private bool destroyed = false;
    private CrystalManager manager;

    void Start()
    {
        manager = FindObjectOfType<CrystalManager>();
    }

    public void BreakCrystal()
    {
        if (destroyed) return;
        destroyed = true;

        if (manager != null)
        {
            manager.NotifyCrystalDestroyed(this.gameObject);
        }

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
