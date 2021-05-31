using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [Range (0f, 5f)]
    public float Sight = 1;
    [Range (0f, 5f)]
    public float Speed = 1;
    [Range(0f, 5f)]
    public float SeparationStrength = 1;
    [Range(0f, 5f)]
    public float AlignmentStrength = 1;
    [Range(0f, 5f)]
    public float CohesionStrength = 1;
    [Range(0f, 5f)]
    public float maxSteerForce = 1;
    [Range(0f, 5f)]
    public float obstacleAvoidanceStrength = 1;
    public Sprite sprite;

    public int FlockSize = 10;
    private GameObject[] boids;

    private void OnValidate()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        boids = new GameObject[FlockSize];
        for (int i = 0; i < FlockSize; i++)
        {
            boids[i] = CreateBoid(sprite, this.GetComponent<Transform>());
        }

        boids[0].GetComponent<SpriteRenderer>().color = new Color(1f, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public GameObject CreateBoid(Sprite sprite, Transform parent)
    {
        //Creates the boid object
        GameObject boid = new GameObject("Boid");

        //add the movement controller
        boid.AddComponent<MovementController>();
        //add and set the sprite renderer
        SpriteRenderer spriteRenderer = boid.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        //add the collider and rigidbody
        CapsuleCollider2D colliderSmall = boid.AddComponent<CapsuleCollider2D>();
        colliderSmall.offset = new Vector2(0f, .09f);
        colliderSmall.size = new Vector2(1f, .75f);

        Rigidbody2D rigidBody = boid.AddComponent<Rigidbody2D>();
        rigidBody.gravityScale = 0;

        //rescale the image
        boid.transform.localScale = new Vector3(0.25f, .5f, .5f);
        boid.transform.SetParent(parent);

        return boid;
    }
}
