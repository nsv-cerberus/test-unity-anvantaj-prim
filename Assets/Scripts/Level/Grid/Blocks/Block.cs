using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public enum BlockMoveDirection
{
    Left,
    Right,
    Up,
    Down
}

[RequireComponent(typeof(BoxCollider2D))]
public class Block : PoolObject
{
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _moveAnimationDuration = 0.5f;
    [SerializeField] private float _mergeAnimationDuration = 0.5f;

    [Inject] private LevelGameplayManager _levelGameplayManager;

    private BoxCollider2D _boxCollider;
    private int _number;
    private int[] _gridPosition = new int[2];
    private Blocks _blocks;
    private bool _inMotion = false;

    public int Number => _number;
    public int RowPosition => _gridPosition[0];
    public int ColumnPosition => _gridPosition[1];
    public bool InMotion => _inMotion;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    public void Initialize(int number, Color color, int columnIndex, Blocks blocks)
    {
        _number = number;
        _text.text = number.ToString();
        _background.color = color;
        SetGridPosition(0, columnIndex);
        _blocks = blocks;

        transform.localPosition = _blocks.Grid.GetCellPosition(0, columnIndex);
        _boxCollider.enabled = true;

        _levelGameplayManager.MoveBlocksDown += MoveBlockDown;
    }

    private void MoveBlockDown()
    {
        MoveBlockToDirection(BlockMoveDirection.Down);
    }

    public void SetGridPosition(int rowPosition, int columnPosition)
    {
        _gridPosition[0] = rowPosition;
        _gridPosition[1] = columnPosition;
    }

    public void TryMoveBlockToDirection(BlockMoveDirection direction)
    {
        bool canMove = direction switch
        {
            BlockMoveDirection.Left => _blocks.Grid.CheckLeftNeighbourIsEmpty(RowPosition, ColumnPosition),
            BlockMoveDirection.Right => _blocks.Grid.CheckRightNeighbourIsEmpty(RowPosition, ColumnPosition),
            BlockMoveDirection.Up => _blocks.Grid.CheckUpNeighbourIsEmpty(RowPosition, ColumnPosition),
            BlockMoveDirection.Down => _blocks.Grid.CheckDownNeighbourIsEmpty(RowPosition, ColumnPosition),
            _ => false
        };

        if (canMove)
        {
            MoveBlockToDirection(direction, CheckBlockToMerge);
        }
    }

    private void CheckBlockToMerge()
    {
        var directions = new (Func<int, int, Block> GetNeighbour, Func<int, int, bool> IsEmpty)[]
        {
            (_blocks.Grid.GetLeftNeighbourBlock, _blocks.Grid.CheckLeftNeighbourIsEmpty),
            (_blocks.Grid.GetUpNeighbourBlock, _blocks.Grid.CheckUpNeighbourIsEmpty),
            (_blocks.Grid.GetRightNeighbourBlock, _blocks.Grid.CheckRightNeighbourIsEmpty),
            (_blocks.Grid.GetDownNeighbourBlock, _blocks.Grid.CheckDownNeighbourIsEmpty)
        };

        foreach (var (getNeighbour, isEmpty) in directions)
        {
            var neighbour = getNeighbour(RowPosition, ColumnPosition);
            if (neighbour != null && !neighbour.InMotion && neighbour.Number == _number)
            {
                MergeWithNeighbour(neighbour);
                return;
            }
        }
    }

    private void MergeWithNeighbour(Block neighbourBlock)
    {
        Vector3 mergePosition = (transform.localPosition + neighbourBlock.transform.localPosition) / 2;

        _blocks.Grid.UnregisterBlockFromCell(this);
        _blocks.Grid.UnregisterBlockFromCell(neighbourBlock);

        PlayMergeAnimation(mergePosition);
        neighbourBlock.PlayMergeAnimation(mergePosition);
    }

    private void MoveBlockToDirection(BlockMoveDirection direction, Action callback = null)
    {
        int[] newPosition = direction switch
        {
            BlockMoveDirection.Left => _blocks.Grid.MoveBlockToLeftCell(this),
            BlockMoveDirection.Right => _blocks.Grid.MoveBlockToRightCell(this),
            BlockMoveDirection.Up => _blocks.Grid.MoveBlockToUpCell(this),
            BlockMoveDirection.Down => _blocks.Grid.MoveBlockToDownCell(this),
            _ => null
        };

        if (newPosition != null)
        {
            SetGridPosition(newPosition[0], newPosition[1]);
            PlayAnimationToHimselfPosition(callback);
        }
    }

    private void PlayAnimationToHimselfPosition(Action callback = null)
    {
        _inMotion = true;
        _levelGameplayManager.IncreaseCountOfBlocksInMotion();

        Vector2 position = _blocks.Grid.GetCellPosition(RowPosition, ColumnPosition);
        transform.DOLocalMove(new Vector3(position.x, position.y, transform.localPosition.z), _moveAnimationDuration)
            .OnComplete(() =>
            {
                _levelGameplayManager.DecreaseCountOfBlocksInMotion();
                _inMotion = false;
                callback?.Invoke();
            });
    }

    private void PlayMergeAnimation(Vector3 mergePosition)
    {
        _levelGameplayManager.MoveBlocksDown -= MoveBlockDown;
        _boxCollider.enabled = false;
        _levelGameplayManager.IncreaseCountOfBlocksInMergeMotion();

        transform.DOLocalMove(mergePosition, _mergeAnimationDuration)
            .OnComplete(() =>
            {
                ReturnToObjectsPool();
                _levelGameplayManager.DecreaseCountOfBlocksInMergeMotion();
            });
    }

    protected override void ReturnToObjectsPool()
    {
        _blocks.BlocksPool.Add(this);
    }
}