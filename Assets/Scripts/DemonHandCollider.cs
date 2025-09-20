using UnityEngine;

public class DemonHandCollider : MonoBehaviour
{
    public PlayerHealth playerHealth; // assign in inspector
    public int damage = 25;
    private bool hasDealtDamage = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasDealtDamage && playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            hasDealtDamage = true;
            Debug.Log("Player hit!");
        }
    }

    public void ResetDamageFlag()
    {
        hasDealtDamage = false;
    }
}
