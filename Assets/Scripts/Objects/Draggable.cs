using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// Performs object movement operations including user input dragging
/// and dropping the object at specified position.
/// 
/// </summary>
/// <remarks>
/// When enabled, the user can drag the object.
/// </remarks>
public class Draggable : MonoBehaviour
{
    private const float TapThreshold = 0.1f; // world units the pointer can move and still count as a tap

    // ================================
    // Events
    // ================================

    public event Action StartDrag;

    /// <summary>
	/// Invoked when the user releases the player.
	/// </summary>
    public event Action<Vector3> Released;

    public event Action Tapped;

    // ================================
    // Private Fields
    // ================================

    private Camera cam;

    private bool selected = false;
    private Vector3 dragOffset;

    private Vector3 pointerStartPos;
    private Vector3 objectStartPos;
    private bool dragged = false;

    private float minX, maxX, minY, maxY;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        cam = Camera.main;
        Debug.Assert(cam != null, "Main camera not found");
    }

    void Update()
    {
        Drag();
    }


    // ================================
    // Initialization
    // ================================

    /// <summary>
	/// Sets the boundaries for dragging the player around the screen.
	/// </summary>
	/// <param name="left">Left boundary.</param>
	/// <param name="right">Right boundary.</param>
	/// <param name="top">Top boundary.</param>
	/// <param name="bottom">Bottom boundary.</param>
    public void Initialize(float left, float right, float top, float bottom)
    {
        minX = left;
        maxX = right;
        minY = bottom;
        maxY = top;

        objectStartPos = transform.position;
    }


    // ================================
    // Public Methods
    // ================================

    /// <summary>
	/// Updates the player's transform position.
	/// </summary>
	/// <param name="newPosition">New position for the player.</param>
    public void Drop(Vector3 newPosition)
    {
        transform.position = newPosition;
        objectStartPos = newPosition;
    }


    // ================================
    // Private Methods
    // ================================

    /// <summary>
	/// Drags and drops the player from user input. Relies on Update().
	/// </summary>
    private void Drag()
    {
        var pointer = Pointer.current;
        if (pointer == null) return;

        Vector2 pointerScreenPos = pointer.position.ReadValue();    // obtain the mouse world coordinates
        Vector3 pointerWorldPos = cam.ScreenToWorldPoint(pointerScreenPos);
        pointerWorldPos.z = 0;

        // start moving
        if (pointer.press.wasPressedThisFrame)
        {
            Collider2D hit = Physics2D.OverlapPoint(pointerWorldPos); // check if mouse is on the collider
            if (hit && hit.gameObject == gameObject)
            {
                selected = true;

                pointerStartPos = pointerWorldPos;      // for distance check
                dragOffset = transform.position - pointerWorldPos;

                StartDrag?.Invoke();
            }
        }

        // continue moving
        if (selected && pointer.press.isPressed)
        {
            float distanceMoved = Vector3.Distance(pointerStartPos, pointerWorldPos);
            if (distanceMoved > TapThreshold)
            {
                dragged = true;
            }

            Vector3 newPos = pointerWorldPos + dragOffset;    // calculate new position

            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);   // clamp position to boundaries
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

            transform.position = newPos;
        }

        // release - checks for tap vs drag
        if (selected && pointer.press.wasReleasedThisFrame)
        {
            selected = false;

            if (dragged)
            {
                Released?.Invoke(transform.position); // existing drag-release event
            }
            else
            {
                transform.position = objectStartPos;    // make sure object stays exactly where it was
                Tapped?.Invoke();               // new event for tap
            }

            dragged = false;
        }
    }
}
