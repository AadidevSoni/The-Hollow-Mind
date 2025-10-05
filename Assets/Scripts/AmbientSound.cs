using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    [Tooltip("Area of the sound to be in")]
    public Collider Area;

    [Tooltip("Character to track")]
    public GameObject Player;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
            audioSource.Play();
    }

    void Update()
    {
        if (Player != null && Area != null)
        {
            Vector3 closestPoint = Area.ClosestPoint(Player.transform.position);
            transform.position = closestPoint;
        }
    }
}
