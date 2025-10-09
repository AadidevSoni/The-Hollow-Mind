using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Ladder : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position, GetComponent<BoxCollider>().size);
    }
}
