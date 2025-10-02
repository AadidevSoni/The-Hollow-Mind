using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 50f; // Degrees per second
    private bool canRotate = true;    // Stop rotation when pickaxe collected

    void Update()
    {
        if (canRotate)
        {
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        }
    }

    // Call this when pickaxe is collected
    public void StopRotation()
    {
        canRotate = false;
    }
}
