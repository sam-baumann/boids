using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private Transform localTransform;
    private Flock flock;

    // Start is called before the first frame update
    void Start()
    {
        //picks a random point on the border of the screen and starts it with a speed
        //but for now only starts it in a random direction
        localTransform = GetComponent<Transform>();
        localTransform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        flock = GetComponentInParent<Flock>();
    }

    // Update is called once per frame
    void Update()
    {
        //start by finding all flockmates in the sight of the current boid

        //disable this object's collider before searching 
        CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        Transform transform = rigidbody2D.transform;
        float totalTurn = 0;
        int wallMask = 1 << 3;

        collider.enabled = false;

        RaycastHit2D[] flockmates = Physics2D.CircleCastAll(transform.position, flock.Sight, Vector2.zero, Mathf.Infinity, 1);
        
        collider.enabled = true;

        int numHits = flockmates.Length;

        rigidbody2D.velocity = Vector2.zero;

        //boid behavior goes here

        if (numHits > 0)
        {
            //the first rule is separation. add all the vectors from neighbors to the current boid to check which direction the boid should turn
            //then, alignment. Find average heading of each of the surrounding boids and turn this one toward that
            //finally, cohesion, find the average position (center of mass) of local flockmates
            Vector2 totalSeparation = new Vector2(0, 0);
            Vector2 avgAlignment = new Vector2(0, 0);
            Vector2 avgPosition = new Vector2(0, 0);
            foreach (RaycastHit2D hit in flockmates)
            {
                totalSeparation += (position2(transform.position) - position2(hit.transform.position));
                avgAlignment += ForwardDir(hit.transform);
                avgPosition += position2(hit.transform.position);
            }

            totalSeparation = totalSeparation.normalized;
            avgAlignment = avgAlignment.normalized;

            avgPosition /= numHits;

            totalTurn += flock.SeparationStrength * TurnToward(transform, totalSeparation);
            totalTurn += flock.AlignmentStrength * TurnToward(transform, avgAlignment);
            totalTurn += flock.CohesionStrength * TurnToward(transform, avgPosition);
        }

        //we also want obstacle avoidance

        collider.enabled = false;

        RaycastHit2D wallHit = Physics2D.CircleCast(transform.position, flock.Sight, Vector2.zero, 0, wallMask);

        collider.enabled = true;

        if (wallHit) totalTurn += flock.obstacleAvoidanceStrength * TurnToward(transform, wallHit.normal);

        //test by drawing lines to the red flockmate
        if (GetComponent<SpriteRenderer>().color == new Color(1f, 0f, 0f))
        {
            foreach (RaycastHit2D b in flockmates)
            {
                Debug.DrawLine(b.transform.position, transform.position);
            }
        }

        //tests if the boid has gone off screen
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (!GeometryUtility.TestPlanesAABB(planes, GetComponent<CapsuleCollider2D>().bounds))
        {
            //transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + 180);
            if (!planes[0].GetSide(transform.position) || !planes[1].GetSide(transform.position)) transform.position = new Vector3(-transform.position.x, transform.position.y);
            if (!planes[2].GetSide(transform.position) || !planes[3].GetSide(transform.position)) transform.position = new Vector3(transform.position.x, -transform.position.y);
        }

        //moves the boid in the forward direction
        //this should go last so it takes any changes in position of direction into account
        totalTurn = Mathf.Clamp(totalTurn, -2, 2);
        transform.eulerAngles += new Vector3(0, 0, totalTurn);
        localTransform.Translate(0.001f, 0.01f * flock.Speed, 0.0f);
    }

    Vector2 ForwardDir(Transform t)
    {
        return new Vector2(Mathf.Cos((t.eulerAngles.z + 90) * Mathf.Deg2Rad), Mathf.Sin((t.eulerAngles.z + 90) * Mathf.Deg2Rad));
    }

    //returns a float between -1 and 1 based on how much it should turn. t is the current transform and n is the vector that should be turned toward
    float TurnToward(Transform t, Vector2 n)
    {
        return Vector2.SignedAngle(ForwardDir(t), n) / 180;
    }

    Vector2 position2(Vector3 position)
    {
        return new Vector2(position.x, position.y);
    }
}
