using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiater : MonoBehaviour
{
    private bool holdsBall = false;
    [SerializeField]
    private float coolDown = 0.8f;
    [SerializeField]
    private float inCoolDown = 0;
    [SerializeField]
    private GameManager GM;
    [SerializeField]
    public float moveSpeed = 1f;
    public Transform leftLimit;
    public Transform rightLimit;
    private bool hasStartedSpawning = false;

    private void Start()
    {
        // Subscribe to login events
        TwitterOAuth.OnLoginCompleted += OnUserLoggedIn;
        GM = GameManager.Instance;
        GM.instantiater = this;
        // Only start spawning once player is logged in
        if (GameManager.IsLoggedIn)
        {
            StartSpawning();
        }
    }

    private void OnUserLoggedIn(string twitterUsername, string walletAddress)
    {
        Debug.Log("Instantiater: User logged in, starting ball spawning");
        StartSpawning();
    }

    public void StartSpawning()
    {
        if (!hasStartedSpawning)
        {
            hasStartedSpawning = true;
            Debug.Log("Instantiater: Starting ball spawning process");
            pickBall();
        }
        else
        {
            Debug.Log("Instantiater: Spawning already started, ignoring duplicate call");
        }
    }


    public void Drop()
    {
        holdsBall = false;
        inCoolDown = coolDown;
    }

    
    private void Update()
    {
        // Do nothing if not logged in yet or spawning hasn't started
        if (!GameManager.IsLoggedIn || !hasStartedSpawning)
            return;

        if (holdsBall == false)
        {
            if (inCoolDown > 0)
                inCoolDown -= Time.deltaTime;
            if (inCoolDown <= 0)
            {
                pickBall();
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        TwitterOAuth.OnLoginCompleted -= OnUserLoggedIn;
    }


    private void pickBall()
    {
        if (GM == null || GM.balls == null || GM.balls.Length == 0)
        {
            Debug.LogError("Instantiater: GameManager or balls array is null/empty!");
            return;
        }

        int index;
        float randValue = Random.value;
        if (randValue < .5f)
        {
            index = 0;
        }
        else if (randValue < .7f) 
        {
            index = 1;
        }
        else if(randValue < .85f)
        {
            index = 2;
        }
        else // 10% of the time
        {
            index = 3;
        }

        Debug.Log($"Instantiater: Creating ball at index {index} (random value: {randValue})");
        GameObject ball = Instantiate(GM.balls[index], transform) as GameObject;
        /*if (GM.shrinkBallSizes > 1)
            ball.transform.localScale /= GM.shrinkBallSizes;
        else if (GM.shrinkBallSizes < 0)
            ball.transform.localScale *= -GM.shrinkBallSizes;*/
        ball.name = GM.balls[index].name;
        
        // Set up rigidbody for kinematic mode
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        
        // Set collision detection mode first, then kinematic
        // This reduces the warning (Unity auto-adjusts collision detection for kinematic bodies)
        ballRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        ballRb.isKinematic = true;
        ballRb.interpolation = RigidbodyInterpolation.None;
        
        ball.transform.parent = this.transform;
        ball.AddComponent<instantiaterChild>();
        ball.GetComponent<instantiaterChild>().instantiater = this;
        ball.GetComponent<TrailRenderer>().startWidth = ball.transform.localScale.x;
        ball.GetComponent<TrailRenderer>().endWidth = (ball.transform.localScale.x / 2f);
        holdsBall = true;
    }
}
