using UnityEngine;
using System.Collections.Generic;
using static BoidSimulator;


public class BoidSimulator : MonoBehaviour
{
    public GameObject boidPrefab;  // Reference to the boid prefab (e.g., a sphere)
    public GameObject cylinderPrefab; // Reference to the cylinder prefab (used for representing boid movement)
    //public float wanderFactor = 0.2f; // Controls how much low-energy boids wander

    private List<Boid> boids = new List<Boid>();
    private List<GameObject> boidObjects = new List<GameObject>(); // To store boid prefab instances
    private List<GameObject> cylinderObjects = new List<GameObject>(); // To store cylinder prefab instances

   public float SplitChance = 0.002f;
    public float  RepulsionRadius = 1f;

    public float wanderStrength = 0.5f;

    public float UpStrength = 0.1f;

    private float MaxEnergy = 100f;

    void Start()
    {
        InitializeSimulation();  // Initialize the simulation with one boid
    }

    void Update()
    {
        // Restart simulation when mouse is pressed
        if (Input.GetMouseButtonDown(0))
        {
            RestartSimulation();
        }

        // Update all boids
        for (int i = boids.Count - 1; i >= 0; i--)
        {
            UpdateBoid(boids[i]);
            ShowBoid(boids[i]);
            boidObjects[i].transform.position = boids[i].Position;

            // Remove boid if it has no energy or is out of bounds
            /*
            if (boids[i].Position.y < -10f || boids[i].Energy <= 0)
            {
                DestroyBoid(i);
            }
            */
        }

        // Draw all previously stored cylinders (using instantiated prefabs)
        foreach (var cyl in cylinderObjects)
        {
            // Each cylinder object is updated based on boid movement
        }

        Debug.Log(boids.Count);
    }


    void CreateBoid(Vector3 pos, float energy)
    {
        boids.Add(new Boid(pos, energy));
        GameObject boidInstance = Instantiate(boidPrefab, pos, Quaternion.identity);
        boidObjects.Add(boidInstance);  // Store boid prefab instances

    }

    void DestroyBoid(int index)
    {

        boids.RemoveAt(index);
        Destroy(boidObjects[index]);
        boidObjects.RemoveAt(index);
    }


    void SplitBoid(Boid boid)
    {
        Vector3 offset = Random.insideUnitSphere * 0.01f;
        CreateBoid(boid.Position + offset, boid.Energy);
        boid.Energy -= 10f;
        //boid.Velocity = Vector3.zero;
    }

    // Method to update boid logic
    void UpdateBoid(Boid boid)
    {

        if ( boid.Energy <= 0 )
        {
            return;
        }
        // Reduce energy over time
        boid.Energy -= 10f * Time.deltaTime;

        // Apply attraction force (towards the top)
        Vector3 attraction = new Vector3(0, UpStrength, 0);
        ApplyForce(boid, attraction);

        // Apply gravity based on total movement distance
        
        float gravityStrength = Mathf.Lerp(0, 0.1f, boid.TotalDistance / 1000f);
        Vector3 gravity = new Vector3(0, gravityStrength, 0);
        ApplyForce(boid, gravity);
        

        // Random wandering
        //float wanderStrength = Mathf.Lerp(wanderFactor, 0.1f, boid.Energy / 100f);
        
        Vector3 wander = new Vector3(Random.Range(-wanderStrength, wanderStrength),
                                     Random.Range(-wanderStrength, wanderStrength),
                                     Random.Range(-wanderStrength, wanderStrength));
        ApplyForce(boid, wander);

        // Repulsion from other boids
        foreach (Boid otherBoid in boids)
        {
            if (!ReferenceEquals(otherBoid, boid))
            {
                Vector3 diff = boid.Position - otherBoid.Position;
                float distance = diff.magnitude;
                if (distance < RepulsionRadius)
                {
                    diff.Normalize();
                    ApplyForce(boid, diff * 0.5f);
                }
            }
        }

        boid.Acceleration *= 0.01f;

        // Update velocity and position
        boid.Velocity += boid.Acceleration;
        /*
        float maxStep = Mathf.Lerp(1, 10, boid.Energy / 100f); // Limit movement based on energy
        if (boid.Velocity.magnitude > maxStep)
        {
            boid.Velocity = boid.Velocity.normalized * maxStep;
        }
        */
        boid.TotalDistance += boid.Velocity.magnitude;
        boid.Position += boid.Velocity * Time.deltaTime;
        boid.Acceleration = Vector3.zero;

 
        // Create a new boid by splitting (random chance)
        if (Random.value < SplitChance)
        {
            SplitBoid(boid);
            
        }

    }

    // Method to apply forces to boids
    void ApplyForce(Boid boid, Vector3 force)
    {
        boid.Acceleration += force;
    }

    // Method to show the boid's movement (using prefabs)
    void ShowBoid(Boid boid)
    {

        if (boid.Energy <= 0)
        {
            return;
        }
        // Store cylinder information and create cylinder prefab in Unity
        Vector3 direction = boid.Position - boid.PreviousPosition;
        float length = direction.magnitude;
        direction.Normalize();

        // Instantiate a cylinder at the boid's previous position
        
        GameObject newCylinder = Instantiate(cylinderPrefab, boid.PreviousPosition, Quaternion.identity);
        float diameter = Mathf.Lerp(0.01f, 0.5f, boid.Energy / MaxEnergy);
        newCylinder.transform.localScale = new Vector3(diameter, length/2.0f , diameter); // Scale based on length
        newCylinder.transform.LookAt(boid.Position); // Point cylinder towards boid
        newCylinder.transform.Rotate(new Vector3(90,0,0));

        cylinderObjects.Add(newCylinder); // Store the cylinder object for future reference
            
        boid.PreviousPosition = boid.Position; // Update the previous position for next frame
    }

    // Initialize simulation with a single boid
    void InitializeSimulation()
    {
        //boids.Add(new Boid(Vector3.zero, 100f));
        boidObjects.Clear();
        cylinderObjects.Clear();
        CreateBoid(Vector3.zero, MaxEnergy);
        
    }

    // Restart the simulation
    void RestartSimulation()
    {
        // Destroy all boid and cylinder prefabs
        foreach (var boidObj in boidObjects)
        {
            Destroy(boidObj);
        }

        foreach (var cylinderObj in cylinderObjects)
        {
            Destroy(cylinderObj);
        }

        // Clear the lists
        boids.Clear();
        boidObjects.Clear();
        cylinderObjects.Clear();

        // Reinitialize with a new set of boids
        InitializeSimulation();
    }

    // Boid structure now only holds the data
    public class Boid
    {
        public Vector3 Position;
        public Vector3 PreviousPosition;
        public Vector3 Velocity;
        public Vector3 Acceleration;
        public float Energy;
        public float TotalDistance;
        public float SplitChance;
        public float RepulsionRadius;

        public Boid(Vector3 initialPosition, float initialEnergy)
        {
            Position = initialPosition;
            PreviousPosition = initialPosition;
            Velocity = Vector3.zero; // Initial downward movement
            Acceleration = Vector3.zero;
            Energy = initialEnergy;
            TotalDistance = 0;

        }
    }

}
