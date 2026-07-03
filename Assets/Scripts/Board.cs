using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    // Constants
    private const int NumRows = 8;
    private const int NumCols = 6;

    // Public Fields
    public static Board Instance;

    // Inspector Fields
    [SerializeField] private MoveableObject[] moveableObjects = new MoveableObject[NumRows * NumCols]; // holds references to all objects currently on the board

    // Private Fields
    private BoardGeometry boardGeometry;

    // Unity Lifecycle
    void Awake()
    {
        Instance = this;
        boardGeometry = new BoardGeometry(NumRows, NumCols, GetComponent<SpriteRenderer>().bounds, transform.position);

        // Subscribe to all the currently set moveable objects
        foreach (MoveableObject obj in moveableObjects)
        {
            if (obj != null)
                obj.MovObjReleased += HandleMovObjReleased;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to all the currently set moveable objects
        foreach (MoveableObject obj in moveableObjects)
        {
            if (obj != null)
                obj.MovObjReleased -= HandleMovObjReleased;
        }
    }


    // Event Handlers
    private void HandleMovObjReleased(MoveableObject obj, Vector3 position)
    {
        Vector2Int index = boardGeometry.TransformToBoardIndex(position);

        // check and see if that index is filled

        Vector3 newPosition = boardGeometry.BoardIndexToTransform(index);

        obj.DropOnPosition(index, newPosition);
    }

}
