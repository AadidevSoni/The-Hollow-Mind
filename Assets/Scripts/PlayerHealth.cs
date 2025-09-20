using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    int hp;

    void Awake() { hp = maxHP; }

    public void TakeDamage(int amount)
    {
        hp -= amount;
        Debug.Log($"Player hit: -{amount}, hp={hp}");
        if (hp <= 0) Die();
    }

    void Die()
    {
        Debug.Log("Player died");
        // play death, end game, etc.
    }
}
