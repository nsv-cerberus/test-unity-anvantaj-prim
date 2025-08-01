using UnityEngine;
using Zenject;

public struct Cell
{
    private Vector3 _position;
    private Block _block;

    public Vector3 Position => _position;
    public Block Block => _block;

    public void SetPosition(Vector3 position) => _position = position;
    public void SetBlock(Block block) => _block = block;
}

public class Grid : MonoBehaviour
{
    [Inject] private LevelGameplayManager _levelGameplayManager;

    private const int _rowsCount = 9;
    private const int _columnsCount = 5;
    private Cell[,] _cells;

    public Cell[,] Cells => _cells;

    private void Awake()
    {
        _cells = new Cell[_rowsCount, _columnsCount];

        _levelGameplayManager.CheckGameOver += CheckGameOver;
}

    public void DefineCellsPositions(float width, float height)
    {
        for (int row = 0; row < _cells.GetLength(0); row++)
        {
            for (int col = 0; col < _cells.GetLength(1); col++)
            {
                float xPosition = col * width;
                float yPosition = row * -height + height;
                _cells[row, col].SetPosition(new Vector3(xPosition, yPosition, 0));
            }
        }

        // LogCellsData();
    }

    /*
    private void LogCellsData()
    {
        Debug.Log("Cells Data: -------------------------");

        for (int row = 0; row < _cells.GetLength(0); row++)
        {
            for (int col = 0; col < _cells.GetLength(1); col++)
            {
                Cell cell = _cells[row, col];
                Debug.Log($"Cell[{row}, {col}]");
                Debug.Log($"Block: {cell.Block}");
                Debug.Log($"Position: {cell.Position}");
            }
        }

        Debug.Log("Cells Data End ---------------------");
    }
    */

    public void RegisterBlockToCell(Block block) => _cells[block.RowPosition, block.ColumnPosition].SetBlock(block);
    public void UnregisterBlockFromCell(Block block) => _cells[block.RowPosition, block.ColumnPosition].SetBlock(null);

    public void MoveBlockToLeftCell(Block block)
    {
        var row = block.RowPosition;
        var col = block.ColumnPosition;
        var newCol = col - 1;

        if (newCol < 0 || _cells[row, newCol].Block != null) return;

        _cells[row, newCol].SetBlock(block);
        _cells[row, col].SetBlock(null);

        block.SetGridPosition(row, newCol);
    }

    public void MoveBlockToRightCell(Block block)
    {
        var row = block.RowPosition;
        var col = block.ColumnPosition;
        var newCol = col + 1;

        if (newCol >= _cells.GetLength(1) || _cells[row, newCol].Block != null) return;

        _cells[row, newCol].SetBlock(block);
        _cells[row, col].SetBlock(null);

        block.SetGridPosition(row, newCol);
    }

    public void MoveBlockToUpCell(Block block)
    {
        var row = block.RowPosition;
        var col = block.ColumnPosition;
        var newRow = row - 1;

        if (newRow < 1 || _cells[newRow, col].Block != null) return;

        _cells[newRow, col].SetBlock(block);
        _cells[row, col].SetBlock(null);

        block.SetGridPosition(newRow, col);
    }

    public void MoveBlockToDownCell(Block block)
    {
        var row = block.RowPosition;
        var col = block.ColumnPosition;
        var newRow = row + 1;

        if (newRow >= _cells.GetLength(0) || _cells[newRow, col].Block != null) return;

        _cells[newRow, col].SetBlock(block);
        _cells[row, col].SetBlock(null);

        block.SetGridPosition(newRow, col);
    }

    private void CheckGameOver()
    {
        Debug.Log("Check Game Over");

        var lastRowIndex = _cells.GetLength(0) - 1;

        for (int col = 0; col < _cells.GetLength(1); col++)
        {
            if (_cells[lastRowIndex, col].Block != null)
            {
                Debug.Log("Game Over!");
                _levelGameplayManager.OnGameOver();
                return;
            }
        }
    }

    public Vector3 GetCellPosition(int row, int col) => _cells[row, col].Position;

    public bool CheckLeftNeighbourIsEmpty(int row, int col) => col > 0 && _cells[row, col - 1].Block == null;
    public bool CheckRightNeighbourIsEmpty(int row, int col) => col < _cells.GetLength(1) - 1 && _cells[row, col + 1].Block == null;
    public bool CheckUpNeighbourIsEmpty(int row, int col) => row > 1 && _cells[row - 1, col].Block == null;
    public bool CheckDownNeighbourIsEmpty(int row, int col) => row < _cells.GetLength(0) - 1 && _cells[row + 1, col].Block == null;
        
    public Block GetLeftNeighbourBlock(int row, int col) => col > 0 ? _cells[row, col - 1].Block : null;
    public Block GetRightNeighbourBlock(int row, int col) => col < _cells.GetLength(1) - 1 ? _cells[row, col + 1].Block : null;
    public Block GetUpNeighbourBlock(int row, int col) => row < _cells.GetLength(0) - 1 ? _cells[row + 1, col].Block : null;
    public Block GetDownNeighbourBlock(int row, int col) => row > 0 ? _cells[row - 1, col].Block : null;

    private void OnDisable()
    {
        _levelGameplayManager.CheckGameOver -= CheckGameOver;
    }
}