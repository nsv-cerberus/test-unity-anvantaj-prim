using System;
using Zenject;
using UnityEngine;
using System.Collections.Generic;

public class LevelGameplayManager : IInitializable
{
    public event Action LaunchGameplay;
    public event Action CreateNewRow;
    public event Action MoveBlocksDown;
    public event Action ActivateSensor;
    public event Action<Vector2> LaunchBallToDirection;
    public event Action<Vector2> StopBallOnPosition;

    private Stack<Block> _blocksAnimations = new Stack<Block>();
    private int _move = 0;

    public int Move => _move;

    public void Initialize()
    {
        StartNewMove();
    }

    public void StartNewMove()
    {
        _move++;
        OnCreateNewRow();
        OnActivateSensor();
    }

    public void RegisterBlockAnimation(Block block)
    {
        _blocksAnimations.Push(block);
    }

    public void NotifyBlockAnimationComplete(Block block)
    {
        if (_blocksAnimations.Contains(block))
        {
            _blocksAnimations.Pop();
        }

        if (_blocksAnimations.Count == 0)
        {
            OnActivateSensor();
        }
    }

    public void OnLaunchGameplay() => LaunchGameplay?.Invoke(); 
    public void OnCreateNewRow() => CreateNewRow?.Invoke();
    public void OnMoveBlocksDown() => MoveBlocksDown?.Invoke();
    public void OnActivateSensor() => ActivateSensor?.Invoke();
    public void OnLaunchBallToDirection(Vector2 directionPosition) => LaunchBallToDirection?.Invoke(directionPosition);
    public void OnStopBallOnPosition(Vector2 position) => StopBallOnPosition?.Invoke(position);

}
