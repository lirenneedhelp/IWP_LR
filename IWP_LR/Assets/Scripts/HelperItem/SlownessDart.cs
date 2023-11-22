using UnityEngine;

public class SlownessDart : HelperItem
{
    [SerializeField] Camera cam;
    public float shootForce = 10f;
    public GameObject camHolder;

    public override void Use()
    {
        ShootDart();
    }

    void ShootDart()
    {
        // Raycast from the camera's crosshair
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        
        Vector3 shootDirection = ray.direction;
        shootDirection.Normalize();

        GameObject dart = DartManager.Instance.Instantiate(ray.origin, Quaternion.LookRotation(shootDirection));
        Rigidbody dartRb = dart.GetComponent<Rigidbody>();

        if (dartRb != null)
        {
            dartRb.AddForce(shootDirection * shootForce, ForceMode.Impulse);
        }
        
    }

    
}
