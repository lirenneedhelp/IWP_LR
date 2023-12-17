using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemType : MonoBehaviour
{
    public Item item;

    [SerializeField] float moveSpeed = 1f; // Adjust the movement speed as needed
    [SerializeField] float moveDistance = 0.1f; // Adjust the movement distance as needed
    [SerializeField] float heightAboveFloor = 0.1f;
    [SerializeField] float easeOutFactor = 2.0f; // Adjust this to control the ease-out effect

    float storeY = 0f;
    private void Start()
    {
        // Set the initial position based on the initial height offset
        storeY = heightAboveFloor + transform.position.y;
        //transform.position = new Vector3(transform.position.x, storeY, transform.position.z);
       
    }

    private void Update()
    {
        float pingPongValue = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        float easedValue = Mathf.Pow(pingPongValue / moveDistance, easeOutFactor);
        float newY = storeY - easedValue * moveDistance;

        Vector3 newPosition = new Vector3(transform.position.x, newY, transform.position.z);
        transform.position = newPosition;
    }
}
