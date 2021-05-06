using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidGenerator : MonoBehaviour
{
    public GameObject boid;

    public float spawnRadius = 1.0f;

    public bool drawSpawnRadius = true;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < Constants.numberOfBoids; ++i)
        {
            GameObject boidObject = Instantiate(boid, transform.position + Random.insideUnitSphere * spawnRadius, Random.rotation);
            Boid _boid = boidObject.GetComponent<Boid>();
            _boid.boidID = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (drawSpawnRadius)
        {
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
