using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [Header("Boid Properties")]
    public Vector3 velocity = Vector3.zero;
    public float maxSpeed = 1.0f;
    public float smoothTurning = 10.0f;
    public int boidID = -1;

    [Header("Detecting range")]
    public float cohensionRadius = 10.0f;
    public float seperationRadius = 3.0f;
    public float alignmentRadius = 10.0f;

    [Header("Gizmos Radius")]
    public bool drawCohensionRadius = true;
    public bool drawSeperationRadius = true;
    public bool drawAlignmentRadius = true;


    [Header("Containment Properties")]
    public float boucingFactor = 10.0f;
    public float minX = 0f;
    public float maxX = 0f;
    public float minY = 0f;
    public float maxY = 0f;
    public float minZ = 0f;
    public float maxZ = 0f;

    [Header("Boids LineRenderer")]
    public LineRenderer cohensionLine;
    public LineRenderer seperationLine;
    public LineRenderer alignmentLine;
    public LineRenderer velocityLine;


    private Renderer b_renderer;
    // Start is called before the first frame update
    void Start()
    {
        velocity = transform.forward * maxSpeed;

        b_renderer = GetComponent<Renderer>();

        cohensionLine.enabled = false;
        seperationLine.enabled = false;
        alignmentLine.enabled = false;
        velocityLine.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cohenV = Cohension();
        Vector3 seperateV = Seperation();
        Vector3 alignV = Alignment();
        Vector3 containmentV = Containment();

        //multiply v component to its factor to adjust its effect on the final v
        cohenV *= Constants.cohensionFactor;
        seperateV *= Constants.seperationFactor;
        alignV *= Constants.alignmentFactor;

        //calculate the v sum
        velocity += transform.forward + cohenV + seperateV + alignV + containmentV;
        
        Move(velocity);

        HandleDrawing(cohenV, seperateV, alignV, velocity);
    }

    void HandleDrawing(Vector3 cohenV, Vector3 seperationV, Vector3 alignmentV, Vector3 _velocity)
    {
        if (!Constants.drawingOnSingleBoidOnly)
        {
            b_renderer.material.SetColor("_Color", Color.white);

            if (Constants.drawingVelocityComponents)
            {
                cohensionLine.enabled = true;
                seperationLine.enabled = true;
                alignmentLine.enabled = true;

                cohensionLine.positionCount = 2;
                seperationLine.positionCount = 2;
                alignmentLine.positionCount = 2;

                cohensionLine.SetPosition(1, cohenV.normalized);
                cohensionLine.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x * -1, gameObject.transform.rotation.y * -1, gameObject.transform.rotation.z * -1);

                seperationLine.SetPosition(1, seperationV.normalized);
                seperationLine.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x * -1, gameObject.transform.rotation.y * -1, gameObject.transform.rotation.z * -1);

                alignmentLine.SetPosition(1, alignmentV.normalized);
                alignmentLine.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x * -1, gameObject.transform.rotation.y * -1, gameObject.transform.rotation.z * -1);
            }
            else
            {
                cohensionLine.enabled = false;
                seperationLine.enabled = false;
                alignmentLine.enabled = false;
            }

            if (Constants.drawingVelocitySum)
            {
                velocityLine.enabled = true;

                velocityLine.positionCount = 2;

                velocityLine.SetPosition(1, _velocity.normalized);
                velocityLine.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x * -1, gameObject.transform.rotation.y * -1, gameObject.transform.rotation.z * -1);
            }
            else
            {
                velocityLine.enabled = false;
            }
        }
        else
        {
            if(Constants.singleBoidID == boidID)
            {
                b_renderer.material.SetColor("_Color", Color.red);

                if (Constants.drawingVelocityComponents)
                {
                    cohensionLine.enabled = true;
                    seperationLine.enabled = true;
                    alignmentLine.enabled = true;

                    cohensionLine.positionCount = 2;
                    seperationLine.positionCount = 2;
                    alignmentLine.positionCount = 2;

                    cohensionLine.SetPosition(1, cohenV.normalized);
                    cohensionLine.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x * -1, gameObject.transform.rotation.y * -1, gameObject.transform.rotation.z * -1);

                    seperationLine.SetPosition(1, seperationV.normalized);
                    seperationLine.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x * -1, gameObject.transform.rotation.y * -1, gameObject.transform.rotation.z * -1);

                    alignmentLine.SetPosition(1, alignmentV.normalized);
                    alignmentLine.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x * -1, gameObject.transform.rotation.y * -1, gameObject.transform.rotation.z * -1);
                }
                else
                {
                    cohensionLine.enabled = false;
                    seperationLine.enabled = false;
                    alignmentLine.enabled = false;
                }

                if (Constants.drawingVelocitySum)
                {
                    velocityLine.enabled = true;

                    velocityLine.positionCount = 2;

                    velocityLine.SetPosition(1, _velocity.normalized);
                    velocityLine.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x * -1, gameObject.transform.rotation.y * -1, gameObject.transform.rotation.z * -1);
                }
                else
                {
                    velocityLine.enabled = false;
                }
            }
            else
            {
                cohensionLine.enabled = false;
                seperationLine.enabled = false;
                alignmentLine.enabled = false;
                velocityLine.enabled = false;
            }
        }
    }

    Vector3 limit_speed(Vector3 _v)
    {
        if(_v.magnitude > maxSpeed)
        {
            _v = _v.normalized * maxSpeed;
        }

        return _v;
    }

    void Move(Vector3 _v)
    {
        _v = limit_speed(_v);

        transform.position += _v * Time.deltaTime;

       // Quaternion newRot = Quaternion.Euler(_v.normalized);
        Quaternion newRot = Quaternion.LookRotation(_v);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRot, smoothTurning * Time.deltaTime);
    }

    Vector3 steerTowardTarget(Vector3 target)
    {
        Vector3 desired = target - transform.position;

        desired = desired.normalized;
        desired *= maxSpeed;

        Vector3 steer = desired - velocity;

        return steer;
    }

    Vector3 Cohension()
    {
        Vector3 v = Vector3.zero;

        //how many boids in range
        int count = 0;

        Boid[] boids = FindObjectsOfType<Boid>();
        int totalBoids = boids.Length;

        for(int i = 0; i < totalBoids; ++i)
        {
            Vector3 diff = boids[i].transform.position - transform.position;

            if(diff.magnitude > 0 && diff.magnitude < cohensionRadius)
            {
                v += boids[i].transform.position;
                ++count;
            }
        }

        if(count > 0)
        {
            v /= count;
            Vector3 steer = steerTowardTarget(v);

            return steer;
        }

        return v;
    }

    Vector3 Seperation()
    {
        Vector3 v = Vector3.zero;

        Boid[] boids = FindObjectsOfType<Boid>();
        int totalBoids = boids.Length;

        //how many boids in range
        int count = 0;

        for(int i = 0; i < totalBoids; ++i)
        {
            float distance = Vector3.Distance(boids[i].transform.position, transform.position);

            if(distance > 0 && distance < seperationRadius)
            {
                Vector3 diff = boids[i].transform.position - transform.position;

                diff = diff.normalized;
                diff /= distance;

                v -= diff;

                ++count;
            }
        }

        if(count > 0)
        {
            v /= count;    
        }

        return v;
    }

    Vector3 Alignment()
    {
        Vector3 v = Vector3.zero;

        //how many boids in range
        int count = 0;

        Boid[] boids = FindObjectsOfType<Boid>();
        int totalBoids = boids.Length;

        for (int i = 0; i < totalBoids; ++i)
        {
            Vector3 diff = boids[i].transform.position - transform.position;

            if (diff.magnitude > 0 && diff.magnitude < cohensionRadius)
            {
                v += boids[i].velocity;
                ++count;
            }
        }

        if (count > 0)
        {
            v /= count;

            v = limit_speed(v);

            Vector3 steer = v - velocity;

            steer = limit_speed(steer);

            return steer;
        }

        return v;
    }

    Vector3 Containment()
    {
        Vector3 v = Vector3.zero;

        if(transform.position.x < minX)
        {
            v.x = boucingFactor;
        }else if(transform.position.x > maxX)
        {
            v.x = -boucingFactor;
        }

        if (transform.position.y < minY)
        {
            v.y = boucingFactor;
        }
        else if (transform.position.y > maxY)
        {
            v.y = -boucingFactor;
        }

        if (transform.position.z < minZ)
        {
            v.z = boucingFactor;
        }
        else if (transform.position.z > maxZ)
        {
            v.z = -boucingFactor;
        }

        return v;
    }

    private void OnDrawGizmos()
    {
        if (drawCohensionRadius)
        {
            Gizmos.DrawWireSphere(transform.position, cohensionRadius);
        }

        if (drawSeperationRadius)
        {
            Gizmos.DrawWireSphere(transform.position, seperationRadius);
        }

        if (drawAlignmentRadius)
        {
            Gizmos.DrawWireSphere(transform.position, alignmentRadius);
        }
    }
}
