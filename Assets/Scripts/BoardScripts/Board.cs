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
            if (obj == null) continue;

            obj.MovObjReleased += HandleMovObjReleased;
            if (obj.Type == MoveableType.Spawner)
                ((Spawner)obj).ItemSpawned += HandleItemSpawned;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to all the currently set moveable objects
        foreach (MoveableObject obj in moveableObjects)
        {
            if (obj == null) continue;

            obj.MovObjReleased -= HandleMovObjReleased;
            if (obj.Type == MoveableType.Spawner)
                ((Spawner)obj).ItemSpawned -= HandleItemSpawned;
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
            MoveableObject swappedObj = moveableObjects[flatIndex];

            // put the current object at "this"  index
            Vector2Int oldIndex = obj.Index;
            AddObjectToBoard(swappedObj, oldIndex);
        }

        // put object in its new spot
        AddObjectToBoard(obj, index);
    }

    private void HandleItemSpawned(Item item)
    {
        Debug.Log("item spawned on board");

        // get the closest empty index
        Vector2Int index = Vector2Int.zero;

        
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

    // adds the object to the array and tells the object where to go
    private void AddObjectToBoard(MoveableObject obj, Vector2Int index)
    {
        int flatIndex = TwoDimToFlatIndex(index);
        Vector3 newPosition = boardGeometry.BoardIndexToTransform(index);

        obj.DropOnPosition(index, newPosition);
        moveableObjects[flatIndex] = obj;
    }
}
