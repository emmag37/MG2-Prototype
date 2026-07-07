using UnityEngine;
using System;

public enum MoveableType
{
    Spawner,
    Item,
    PowerUp
}

public enum VariantType
{
    Energy,
    NoEnergy
}

// requires components:
// draggable script
// grid grandparent
// 2d collider
// sprite renderer?
public class MoveableObject : MonoBehaviour
{
    // Constants
    private const int BaseOrder = 1;
    private const int MovingOrder = 2;

    // Public Fields
    public Vector2Int Index;
    public MoveableType Type;

    public bool Moveable = true;   // only works if set in compile time

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
            gridBounds.min.y + spriteRadius,
            Moveable
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

        SetSortingOrder(BaseOrder);
    }

    // Event Handlers
    protected virtual void HandleStartDrag()
    {
        SetSortingOrder(MovingOrder);
    }

    protected virtual void HandleReleased(Vector3 position)
    {
        MovObjReleased?.Invoke(this, position);
    }

    protected virtual void HandleTap()
    {
        // potentially selects the item

        SetSortingOrder(BaseOrder);
    }

    // Private Helper Methods
    protected virtual void SetSortingOrder(int order)
    {
        spriteRenderer.sortingOrder = order;
    }
}
