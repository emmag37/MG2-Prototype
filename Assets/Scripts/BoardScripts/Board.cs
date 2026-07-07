using UnityEngine;
using System.Collections.Generic;
using TMPro;

// need to define board "dead" zones
public class Board : MonoBehaviour
{
    // Constants
    private const int NumRows = 8;
    private const int NumCols = 6;
    private const int StartEnergy = 100;

    static readonly int[] DeadZone = { 4, 5, 11, 36, 42, 43 };
    static readonly Vector2Int NegativeIdx = new Vector2Int(-1, -1);


    // Inspector Fields
    [SerializeField] private TextMeshProUGUI energyText;

    [SerializeField] private MoveableObject[] moveableObjects = new MoveableObject[NumRows * NumCols]; // holds references to all objects currently on the board

    // Private Fields
    private BoardGeometry boardGeometry;
    private int numEnergy;

    // Unity Lifecycle
    void Awake()
    {
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

        // update ui
        UpdateNumEnergy(StartEnergy);
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
        // set old spot to null first
        moveableObjects[TwoDimToFlatIndex(obj.Index)] = null;

        Vector2Int index = boardGeometry.TransformToBoardIndex(position);
        int flatIndex = TwoDimToFlatIndex(index);

        // check for merge/swap objects if the index is filled
        if (IndexInDeadZone(flatIndex))
        {
            Debug.Log("identified dead zone");
            index = obj.Index;
        }
        else if (moveableObjects[flatIndex] != null && moveableObjects[flatIndex] != obj)
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
            {
                AddObjectToBoard(currentObj, obj.Index);    // swap object places
            }
        }

        // put object in its new spot
        AddObjectToBoard(obj, index);
    }

    private void HandleItemSpawned(Item item)
    {
        Vector2Int index = GetEmptyIndex();

        if (index == NegativeIdx || (item.Variant == VariantType.Energy && numEnergy == 0))
        {
            Destroy(item.gameObject);
            return;
        }

        item.MovObjReleased += HandleMovObjReleased;
        AddObjectToBoard(item, index);

        if (item.Variant == VariantType.Energy)
            UpdateNumEnergy(numEnergy - 1);
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

    // currently returns the first empty index, edit to make the closest
    // alg for getting the closest empty index?
    private Vector2Int GetEmptyIndex()
    {
        for (int i = 0;  i < moveableObjects.Length; i++)
        {
            if (moveableObjects[i] == null && !IndexInDeadZone(i)) // bad check, O(n^2), doesn't matter for this
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

    // takes only the flat index
    private bool IndexInDeadZone(int index)
    {
        foreach (int deadIdx in DeadZone)
        {
            if (index == deadIdx) return true;
        }

        return false;
    }

    // updates the num energy and energy text display
    private void UpdateNumEnergy(int num)
    {
        numEnergy = num;
        energyText.text = $"{num}";
    }
}
