using UnityEngine;

public class Board : MonoBehaviour
{
    // Public Fields
    public static Board Instance;

    // Private Fields
    private BoardGeometry boardGeometry;

    // Unity Lifecycle
    void Awake()
    {
        Instance = this;
        boardGeometry = new BoardGeometry(8, 6, GetComponent<SpriteRenderer>().bounds, transform.position);
    }

    // Public Methods

    // takes the postion local to the board
    public (int, int) GetIndex(Vector3 position)
    {
        return boardGeometry.TransformToBoardIndex(position);
    }

    // returns the position local to the board
    public Vector3 GetPosition((int, int) index)
    {
        return boardGeometry.BoardIndexToTransform(index);
    }
}
