using UnityEngine;

public class EnableFOnPickup : MonoBehaviour
{
    public Transform player;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (player == null) return;

            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= 3f)
            {
                if (FKeyManager.Instance != null)
                    FKeyManager.Instance.EnableFKey();

                Debug.Log("F key is now enabled!");
            }
        }
    }
}
