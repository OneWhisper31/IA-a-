using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    private Vector3 _velocity; public Vector3 velocity { get { return _velocity; } }
    public float maxSpeed;
    [Range(0, 0.1f)]
    public float maxForce;

    [Header("Flocking")]
    public float boidViewRadius;

    public float separationRadius;

    public static List<Fish> allBoids = new List<Fish>();

    [Range(0f, 2.5f)]
    public float separationWeight = 1;
    [Range(0f, 2.5f)]
    public float cohesionWeight = 1;
    [Range(0f, 2.5f)]
    public float alignmentWeight = 1;


    [HideInInspector]public Transform currentFruit;


    void Start()
    {
        allBoids.Add(this);

        Vector3 random = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        AddForce(random.normalized * maxSpeed);
    }

    void Update()
    {
        if ((GameManager.instance.hunter.transform.position - transform.position).magnitude <= boidViewRadius)//hunter is near
            AddForce(Evade());
        else if (currentFruit == null)
        {
            AddForce(Separation() * separationWeight + Alignment() * alignmentWeight + Cohesion() * cohesionWeight);//don't have an fruitTarget

            foreach (Transform fruit in GameManager.instance.fruits)
            {
                if (fruit == null) break;

                if ((fruit.position - transform.position).magnitude <= boidViewRadius)//if is near
                {
                    currentFruit = fruit;
                    break;
                }
            }
        }
        else if (currentFruit != null) {
            AddForce(Arrive());
        }

        transform.position += _velocity * Time.deltaTime;
        transform.forward = _velocity;

        CheckBounds();
    }

    Vector3 Separation()
    {
        Vector3 desired = Vector3.zero;

        foreach (var boid in allBoids)
        {
            if (boid == this) continue;
            if (boid == null) break;

            Vector3 dist = boid.transform.position - transform.position;
            if (dist.magnitude <= boidViewRadius)
            {
                desired += dist;
            }
        }
        if (desired == Vector3.zero) return desired;

        desired *= -1;

        return CalculateSteering(desired);
    }

    Vector3 Alignment()
    {
        Vector3 desired = Vector3.zero;
        int count = 0;
        foreach (var boid in allBoids)
        {
            if (boid == this) continue;
            if (boid == null) break;

            if (Vector3.Distance(transform.position, boid.transform.position) <= boidViewRadius)
            {
                desired += boid._velocity;
                count++;
            }
        }
        if (count == 0) return desired;
        desired /= count;

        return CalculateSteering(desired);
    }

    Vector3 Cohesion()
    {
        Vector3 desired = Vector3.zero;
        int count = 0;
        foreach (var boid in allBoids)
        {
            if (boid == this) continue;
            if (boid == null) break;

            if (Vector3.Distance(transform.position, boid.transform.position) <= boidViewRadius)
            {
                desired += boid.transform.position;
                count++;
            }
        }
        if (count == 0) return desired;
        desired /= count;
        
        desired -= transform.position;

        return CalculateSteering(desired);
    }
    Vector3 Evade() {

        var persuitAgent = GameManager.instance.hunter;

        Vector3 futurePos = persuitAgent.transform.position + persuitAgent.velocity;

        Vector3 desired = futurePos - transform.position;

        return -CalculateSteering(desired);
    }
    Vector3 Arrive(){
        if (currentFruit == null) return Vector3.zero;

        Vector3 desired = currentFruit.position - transform.position;

        if (desired.magnitude <= boidViewRadius)
        {
            float speed = maxSpeed * (desired.magnitude / boidViewRadius);
            desired.Normalize();
            desired *= speed;
            if ((currentFruit.position - transform.position).magnitude <= separationRadius)
            {
                GameManager.instance.DestroyFruits(currentFruit.position);
                currentFruit = null;
                return Separation() * separationWeight + Alignment() * alignmentWeight + Cohesion() * cohesionWeight;//don't have an fruitTarget
            }
        }
        else
        {
            desired.Normalize();
            desired *= maxSpeed;
        }
        
        return CalculateSteering(desired);
    }

    Vector3 CalculateSteering(Vector3 desired)
    {
        return Vector3.ClampMagnitude((desired.normalized * maxSpeed) - _velocity, maxForce);
    }

    void CheckBounds()//check bounds a teleports to the other side
    {
        transform.position = GameManager.instance.SetObjectBoundPosition(transform.position);
    }

    void AddForce(Vector3 force)
    {
        force.y = 0;
        _velocity = Vector3.ClampMagnitude(_velocity + force, maxSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, boidViewRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}
