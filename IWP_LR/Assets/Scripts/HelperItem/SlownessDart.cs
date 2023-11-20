using UnityEngine;
using Photon.Pun;
using System.IO;

public class SlownessDart : HelperItem
{
    public GameObject dartPrefab;
    public float shootForce = 10f;
    public PhotonView pv;

    public override void Use()
    {
        ShootDart();
    }

    void ShootDart()
    {
        // Raycast from the camera's crosshair
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        
        Vector3 shootDirection = ray.direction;
        shootDirection.Normalize();

        GameObject dart = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Dart"), transform.position, Quaternion.LookRotation(shootDirection));
        Rigidbody dartRb = dart.GetComponent<Rigidbody>();

        if (dartRb != null)
        {
            dartRb.AddForce(shootDirection * shootForce, ForceMode.Impulse);
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Handle the impact on the target (e.g., remove a part, trigger an animation)
            Debug.Log("Dart hit the target!");

            // Apply Slowness Effects

            // Destroy the dart
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
