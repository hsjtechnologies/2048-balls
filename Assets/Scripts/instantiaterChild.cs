using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class instantiaterChild : MonoBehaviour
{
    public Instantiater instantiater;
    public Rigidbody rb;

    private Transform leftLimit;
    private Transform rightLimit;
    private Touch touch;
    private LineRenderer lineRenderer;
    private bool isBeingDragged = false;
    private Vector3 lastMousePosition;

    private void Start()
    {
        leftLimit = instantiater.leftLimit;
        rightLimit = instantiater.rightLimit;
        Ball ball = GetComponent<Ball>();
        ball.isLocked = true;
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        lastMousePosition = Input.mousePosition;
    }

    public void Update()
    {
        HandleInput();
        
        // Only draw aim line if LineRenderer still exists
        if (lineRenderer != null)
        {
            DrawAimLine();
        }
    }

    private void HandleInput()
    {
        bool mousePressed = Input.GetMouseButton(0);
        bool touchActive = Input.touchCount > 0;
        
        // Handle touch input (for mobile)
        if (touchActive)
        {
            touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Moved)
            {
                MoveHorizontally(touch.deltaPosition.x);
                isBeingDragged = true;
            }
            
            if (touch.phase == TouchPhase.Ended)
            {
                Drop();
                isBeingDragged = false;
            }
        }
        // Handle mouse input (for web/desktop)
        else if (mousePressed)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            float mouseDeltaX = currentMousePosition.x - lastMousePosition.x;
            MoveHorizontally(mouseDeltaX);
            isBeingDragged = true;
            lastMousePosition = currentMousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isBeingDragged && !touchActive)
            {
                Drop();
            }
            isBeingDragged = false;
            lastMousePosition = Input.mousePosition;
        }
        else
        {
            lastMousePosition = Input.mousePosition;
        }
        
        // Handle keyboard input (works independently)
        float horizontalInput = 0f;
        
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInput = -1f;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInput = 1f;
        }
        
        if (horizontalInput != 0f && !mousePressed && !touchActive)
        {
            MoveHorizontally(horizontalInput * 200f); // 200f is a sensitivity multiplier
        }
        
        // Drop ball with Space or Enter key
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            Drop();
        }
    }

    private void MoveHorizontally(float deltaInput)
    {
        // Apply movement with speed multiplier
        float movement = deltaInput * instantiater.moveSpeed * Time.deltaTime;
        
        transform.position = new Vector3(
            transform.position.x + movement,
            transform.position.y,
            transform.position.z
        );
        
        // Clamp position between limits
        transform.position = new Vector3(
            Mathf.Clamp(
                transform.position.x,
                leftLimit.position.x + transform.localScale.x / 2,
                rightLimit.position.x - transform.localScale.x / 2
            ),
            transform.position.y,
            transform.position.z
        );
    }

    private void DrawAimLine()
    {
        // Check if LineRenderer still exists (it might be destroyed by Ball.cs)
        if (lineRenderer == null)
            return;
            
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hit.point);
        }
    }

    private void Drop()
    {
        instantiater.Drop();
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        GetComponent<Ball>().enabled = true;
        GetComponent<Ball>().isLocked = false;
        transform.parent = null;
        Destroy(GetComponent<instantiaterChild>());
    }
}