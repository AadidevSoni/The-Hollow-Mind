using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 50f;
    private bool canRotate = true;

    void Update()
    {
        if (canRotate)
        {
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        }
    }

    public void StopRotation()
    {
        canRotate = false;
    }
}
