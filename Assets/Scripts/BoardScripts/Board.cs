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
        int flatIndex = TwoDimToFlatIndex(index);

        // swap objects if the index is filled
        if (moveableObjects[flatIndex] != null && moveableObjects[flatIndex] != obj)
        {
            Debug.Log("swapped");

            MoveableObject swappedObj = moveableObjects[flatIndex];

            // put the current object at "this"  index
            Vector2Int oldIndex = obj.Index;
            Vector3 oldPosition = boardGeometry.BoardIndexToTransform(oldIndex);
            swappedObj.DropOnPosition(oldIndex, oldPosition);

            // put in new spot in array
            int oldFlat = TwoDimToFlatIndex(oldIndex);
            moveableObjects[oldFlat] = swappedObj;
        } 

        // put object in its new spot
        Vector3 newPosition = boardGeometry.BoardIndexToTransform(index);
        obj.DropOnPosition(index, newPosition);
        moveableObjects[flatIndex] = obj;
    }

    // Private Methods
    private int TwoDimToFlatIndex(Vector2Int idx)
    {
        return idx.x * NumCols + idx.y;
    }

    private Vector2Int FlatToTwoDimIndex(int idx)
    {
        int r = idx / NumCols;
        int c = idx % NumCols;

        return new Vector2Int(r, c);
    }
}
