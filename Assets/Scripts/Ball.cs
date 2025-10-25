using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public bool isLocked = false;
    private bool hasInstantiated = false;
    private GameObject targetObj;
    private GameManager GM;
    public float leftLimit;
    public float rightLimit;
    private Rigidbody rb;

    private void Start()
    {
        // Try multiple ways to find GameManager
        GM = FindObjectOfType<GameManager>();
        
        if (GM == null)
        {
            // Try finding by name as fallback
            GameObject gmObject = GameObject.Find("GameManager");
            if (gmObject != null)
            {
                GM = gmObject.GetComponent<GameManager>();
            }
        }
        
        if (GM == null)
        {
            Debug.LogError("GameManager not found in scene! Ball will not function properly.");
            // Don't return, let the ball exist but with limited functionality
        }
        else
        {
            Debug.Log("GameManager found successfully for ball: " + gameObject.name);
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on " + gameObject.name);
        }

        try { Destroy(GetComponent<LineRenderer>()); }
        catch (System.Exception) { }
    }

    private void OnTriggerStay(Collider other)
    {
        if (GM == null) return; // Safety check

        if (other.name == gameObject.name)
        {
            if (isLocked == false)
            {
                Ball otherBall = other.gameObject.GetComponent<Ball>();
                if (otherBall == null || otherBall.isLocked == true)
                {
                    return; // Abort, other ball already locked or no Ball component!
                }
                isLocked = true;
                otherBall.isLocked = true;
                targetObj = other.gameObject;
            }
            else
            {
                return;
            }
        }

        if (isLocked == false && rb != null && rb.velocity.magnitude <= 0.1f && other.gameObject.name == "GameOverBarrier")
        {
            GM.GameOver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasInstantiated == false && (other.CompareTag("Ball") || other.CompareTag("Border")) && gameObject.transform.parent == null)
        {
            hasInstantiated = true;
        }
    }

    private void Update()
    {
        if (isLocked == true && targetObj != null && targetObj.transform != null)
        {
            float step = 5f * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetObj.transform.position, step);
        }

        if (isLocked == true && targetObj != null && Vector3.Distance(transform.position, targetObj.transform.position) <= 0.1f)
        {
            instantiateNew();
        }
    }

    private void instantiateNew()
    {
        if (GM == null) return; // Safety check

        Destroy(targetObj);
        GameObject ballToInstantiate = null;

        // Find the current ball index
        int currentIndex = -1;
        for (int i = 0; i < GM.balls.Length; i++)
        {
            if (this.name == GM.balls[i].name)
            {
                currentIndex = i;
                break;
            }
        }

        // Get the next ball (if it exists)
        if (currentIndex != -1 && currentIndex + 1 < GM.balls.Length)
        {
            ballToInstantiate = GM.balls[currentIndex + 1];
        }
        else
        {
            Debug.Log("This is the max value ball can have");
            Destroy(this.gameObject);
            return;
        }

        if (ballToInstantiate != null)
        {
            GameObject ball = Instantiate(ballToInstantiate, this.transform.position, this.transform.rotation);
            ball.name = ballToInstantiate.name;

            TrailRenderer trail = ball.GetComponent<TrailRenderer>();
            if (trail != null)
            {
                trail.startWidth = ball.transform.localScale.x;
                trail.endWidth = ball.transform.localScale.x / 2f;
            }

            Ball ballScript = ball.GetComponent<Ball>();
            if (ballScript != null)
            {
                ballScript.enabled = true;
            }

            GM.Merging(transform.name);
        }

        Destroy(this.gameObject);
    }
}