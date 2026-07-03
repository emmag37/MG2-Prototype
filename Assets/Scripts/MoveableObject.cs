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

    // Event Handlers
    private void HandleStartDrag()
    {
        // set layer to top
        spriteRenderer.sortingOrder = 2;
    }

    private void HandleReleased(Vector3 position)
    {
        // get the index and new position from the board
        //Vector3 localPosition = transform.InverseTransformPoint(position);
        (int, int) index = Board.Instance.GetIndex(position);
        Debug.Log($"index: {index}");

        Vector3 newPosition = Board.Instance.GetPosition(index);

        Index = index;
        movement.Drop(newPosition);

        // set layer back to one
        spriteRenderer.sortingOrder = 1;
    }
}
