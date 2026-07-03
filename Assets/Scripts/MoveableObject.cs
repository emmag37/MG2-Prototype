using UnityEngine;
using System;

// requires components:
    // draggable script
    // grid grandparent
    // 2d collider
    // sprite renderer?
public class MoveableObject : MonoBehaviour
{
    // Public Fields
    public (int, int) Index;

    // Events
    public event Action<MoveableObject> ObjectDropped;

    // Private Fields
    private Draggable movement;
    private SpriteRenderer spriteRenderer;

    // Unity Lifecyle
    void Awake()
    {
        movement = GetComponent<Draggable>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        float spriteRadius = spriteRenderer.bounds.extents.x;
        Bounds gridBounds = transform.parent.parent.GetComponent<SpriteRenderer>().bounds;

        movement.Initialize(
            gridBounds.min.x + spriteRadius,
            gridBounds.max.x - spriteRadius,
            gridBounds.max.y - spriteRadius,
            gridBounds.min.y + spriteRadius
        );

        movement.StartDrag += HandleStartDrag;
        movement.Released += HandleReleased;
    }

    void OnDestroy()
    {
        movement.StartDrag -= HandleStartDrag;
        movement.Released -= HandleReleased;
    }


    // Public Methods
    public void DropOnPosition(Vector3 position, (int, int) index) 
    {
        // set layer back to one
        spriteRenderer.sortingOrder = 1;

        Index = index;
        movement.Drop(position);
    }

    // Event Handlers
    private void HandleStartDrag()
    {
        // set layer to top
        spriteRenderer.sortingOrder = 2;
    }

    private void HandleReleased(Vector3 position)
    {
        // send out an action with this object, then the board directs it to drop
        ObjectDropped?.Invoke(this);
    }
}
