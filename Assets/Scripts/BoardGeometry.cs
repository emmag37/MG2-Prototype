using UnityEngine;

/// <summary>
/// Performs the index and world coordinate calculations of the board.
/// </summary>
public class BoardGeometry
{
    // ==================================================
    // Private Fields
    // ==================================================

    private int numRows;
    private int numCols;

    private float cellWidth;
    public float zeroX;    // adjusted 0 at the left in world coordinates
    public float zeroY;    // adjusted 0 at the top in world coordinates


    // ==================================================
    // Initialization
    // ==================================================

    /// <summary>
	/// Calculates the cell offset and origin world coordinates.
	/// </summary>
	/// <param name="rows">Number of rows on the board.</param>
	/// <param name="cols">Number of cols on the board.</param>
	/// <param name="cellRadius">Radius of a spot on the board.</param>
	/// <param name="board">Boundaries of box that holds the board.</param>
    public BoardGeometry(int rows, int cols, Bounds board, Vector3 boardOrigin)
    {
        numRows = rows;
        numCols = cols;

        float boardLeft = board.min.x;
        float boardTop = board.max.y;
        float boardRight = board.max.x;

        cellWidth = (boardRight - boardLeft) / numCols;
        zeroX = boardLeft + cellWidth / 2;
        zeroY = boardTop - cellWidth / 2;
    }


    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Calculates the board index from a position.
	/// </summary>
	/// <param name="position">Transform in world coordinates</param>
	/// <returns>The board index (row, col) corresponding to the board array.</returns>
    public (int, int) TransformToBoardIndex(Vector3 position)
    {
        int r = Mathf.RoundToInt((zeroY - position.y) / cellWidth);
        int c = Mathf.RoundToInt((position.x - zeroX) / cellWidth);

        return (r, c);
    }

    /// <summary>
	/// Calculates the world position of a cell.
	/// </summary>
	/// <param name="index">Board index of the cell, (row, col).</param>
	/// <returns>The world positon of the cell.</returns>
    public Vector3 BoardIndexToTransform((int, int) index)
    {
        Vector3 newTransform = Vector3.zero;

        newTransform.x = zeroX + index.Item2 * cellWidth;
        newTransform.y = zeroY + index.Item1 * cellWidth;

        return newTransform;
    }
}
