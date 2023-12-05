using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemType : MonoBehaviour
{
    public Item item;

    [SerializeField] float rotationSpeed = 30f; // Adjust the rotation speed as needed
    [SerializeField] float moveSpeed = 1f; // Adjust the movement speed as needed
    [SerializeField] float moveDistance = 0.1f; // Adjust the movement distance as needed
    [SerializeField] float heightAboveFloor = 0.1f;
    [SerializeField] float easeOutFactor = 2.0f; // Adjust this to control the ease-out effect


    private void Update()
    {
        // Rotate the object around its local up axis
        //transform.localEulerAngles += new Vector3(0, rotationSpeed * Time.deltaTime, 0);

        float pingPongValue = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        float easedValue = Mathf.Pow(pingPongValue / moveDistance, easeOutFactor);
        float newY = heightAboveFloor - easedValue * moveDistance; // Invert the direction

        // Keep the existing x and z coordinates
        Vector3 newPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);

        // Apply the new position
        transform.localPosition = newPosition;
    }
}
