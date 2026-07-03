using UnityEngine;
using System;

public enum MoveableType
{
    Spawner,
    Item,
    PowerUp
}

// requires components:
// draggable script
// grid grandparent
// 2d collider
// sprite renderer?
public class MoveableObject : MonoBehaviour
{
    // Public Fields
    public Vector2Int Index;
    public MoveableType Type;

    // Events
    public event Action<MoveableObject, Vector3> MovObjReleased;

    // Private Fields
    private Draggable movement;
    private SpriteRenderer spriteRenderer;

    // Unity Lifecyle
    protected virtual void Awake()
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
        movement.Tapped += HandleTap;
    }

    void OnDestroy()
    {
        movement.StartDrag -= HandleStartDrag;
        movement.Released -= HandleReleased;
        movement.Tapped -= HandleTap;
    }

    // Public Methods
    public void DropOnPosition(Vector2Int index, Vector3 position)
    {
        Index = index;
        movement.Drop(position);

        // set layer back to one
        spriteRenderer.sortingOrder = 1;
    }

    // Event Handlers
    private void HandleStartDrag()
    {
        // set layer to top
        spriteRenderer.sortingOrder = 2;
    }

    private void HandleReleased(Vector3 position)
    {
        Debug.Log("released");
        MovObjReleased?.Invoke(this, position);
    }

    protected virtual void HandleTap()
    {
        Debug.Log("tapped");
    }
}
