using UnityEngine;

public class OrbitingCamera : MonoBehaviour
{
    public float rotationSpeed = 10f;  // Speed of rotation
    private float orbitHeight = 5f;     // Height of the camera
    private float orbitDistance = 10f;  // Distance from the center

    private float angle = 0f;

    public void Start()
    {
        orbitDistance = transform.position.z;
        orbitHeight = transform.position.y;
    }

    void Update()
    {
        // Increment the angle based on time and speed
        angle += rotationSpeed * Time.deltaTime;

        // Convert angle to radians
        float radians = angle * Mathf.Deg2Rad;

        // Calculate the new position
        float x = Mathf.Cos(radians) * orbitDistance;
        float z = Mathf.Sin(radians) * orbitDistance;
        float y = orbitHeight;

        // Set the camera position
        transform.position = new Vector3(x, y, z);

        // Look at the center (0,0,0)
        transform.LookAt(new Vector3(0, orbitHeight, 0));
    }
}
