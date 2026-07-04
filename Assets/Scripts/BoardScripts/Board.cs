using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    // Constants
    private const int NumRows = 8;
    private const int NumCols = 6;

    static readonly Vector2Int NegativeIdx = new Vector2Int(-1, -1);

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
            if (obj.Type == MoveableType.PowerUp)
                ((Powerup)obj).ItemSpawned += HandleItemSpawned;
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
        Debug.Log("handle move object released");

        // set old spot to null first
        moveableObjects[TwoDimToFlatIndex(obj.Index)] = null;

        Vector2Int index = boardGeometry.TransformToBoardIndex(position);
        int flatIndex = TwoDimToFlatIndex(index);

        
        // check for merge/swap objects if the index is filled
        if (moveableObjects[flatIndex] != null && moveableObjects[flatIndex] != obj)
        {
            MoveableObject currentObj = moveableObjects[flatIndex];

            if (MergableObjects(obj, currentObj))
            {
                // "Merge" the moved one
                ((Item)obj).Merge();

                // delete the static one
                Destroy(currentObj.gameObject);
            }
            else
                AddObjectToBoard(currentObj, obj.Index);    // swap object places
        }

        // put object in its new spot
        AddObjectToBoard(obj, index);
    }

    private void HandleItemSpawned(Item item)
    {
        Vector2Int index = GetEmptyIndex();
        if (index == NegativeIdx)
        {
            Destroy(item.gameObject);
            return;
        }

        item.MovObjReleased += HandleMovObjReleased;
        AddObjectToBoard(item, index);
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
        Debug.Log($"add object to board at {index}");
        int flatIndex = TwoDimToFlatIndex(index);

        Vector3 newPosition = boardGeometry.BoardIndexToTransform(index);

        obj.DropOnPosition(index, newPosition);
        moveableObjects[flatIndex] = obj;
    }

    // currently returns the first empty index, edit to make the closest
    // alg for getting the closest empty index?
    private Vector2Int GetEmptyIndex()
    {
        for (int i = 0;  i < moveableObjects.Length; i++)
        {
            if (moveableObjects[i] == null)
            {
                return FlatToTwoDimIndex(i);
            }
        }

        // full board
        Debug.Log("full board");
        return NegativeIdx;
    }

    // implement comparator for real project
    private bool MergableObjects(MoveableObject objMoved, MoveableObject objStatic)
    {
        bool mergable = (objMoved.Type == MoveableType.Item
            && objStatic.Type == MoveableType.Item)
            && ((Item)objMoved).Variant == ((Item)objStatic).Variant
            && ((Item)objMoved).Level == ((Item)objStatic).Level;

        return mergable;
    }
}
