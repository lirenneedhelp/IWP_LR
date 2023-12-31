using UnityEngine;
public class SlownessDart : HelperItem
{
    public Camera cam;
    public float shootForce = 10f;

    public override void Use()
    {
        Debug.Log("Using Dart");
        ShootDart();
    }

    void ShootDart()
    {
        // Raycast from the camera's crosshair
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
    
        Vector3 shootDirection = ray.direction;
        shootDirection.Normalize();

        //Add an upward force by modifying the shootDirection
        Vector3 upwardOffset = Vector3.up * 0.2f; // Adjust the multiplier as needed
        shootDirection += upwardOffset;
        shootDirection.Normalize(); // Ensure the vector remains a unit vector

        GameObject dart = DartManager.Instance.InstantiateDart(ray.origin, Quaternion.LookRotation(shootDirection));
        Rigidbody dartRb = dart.GetComponent<Rigidbody>();

        if (dartRb != null)
        {
            dartRb.AddForce(shootDirection * shootForce, ForceMode.Impulse);
        }
    }

}
