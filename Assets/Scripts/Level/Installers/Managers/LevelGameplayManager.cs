using System;
using Zenject;
using UnityEngine;

public class LevelGameplayManager : IInitializable
{
    public event Action LaunchGameplay;
    public event Action CreateNewRow;
    public event Action MoveBlocksDown;
    public event Action ActivateSensor;
    public event Action<Vector2> LaunchBallToDirection;
    public event Action GameOver;

    private int _move = 0;
    private bool _ballIsMove = false;
    private int _blocksInMotionCount = 0;
    private int _blocksInMergeMotionCount = 0;
    private bool _gameOver = false;

    public int Move => _move;

    public void Initialize()
    {
        // StartNewMove();
    }

    public void StartNewMove()
    {
        if (_gameOver)
        {
            Debug.Log("Game Over!");
            return;
        }

        if (!_ballIsMove && _blocksInMotionCount == 0 && _blocksInMergeMotionCount == 0)
        {
            _move++;
            OnCreateNewRow();
        }
    }

    public void SetBallMoveState(bool isMove)
    {
        _ballIsMove = isMove;        
        StartNewMove();
    }

    public void IncreaseCountOfBlocksInMotion() => _blocksInMotionCount++;

    public void DecreaseCountOfBlocksInMotion()
    {
        if (_blocksInMotionCount > 0) _blocksInMotionCount--;
        if (!_ballIsMove && _blocksInMotionCount == 0) OnActivateSensor();
    }

    public void IncreaseCountOfBlocksInMergeMotion() => _blocksInMergeMotionCount++;

    public void DecreaseCountOfBlocksInMergeMotion()
    {
        if (_blocksInMergeMotionCount > 0) _blocksInMergeMotionCount--;
         StartNewMove();
    }

    public void OnLaunchGameplay() => LaunchGameplay?.Invoke(); 
    public void OnCreateNewRow() => CreateNewRow?.Invoke();
    public void OnMoveBlocksDown() => MoveBlocksDown?.Invoke();
    public void OnActivateSensor() => ActivateSensor?.Invoke();
    public void OnLaunchBallToDirection(Vector2 directionPosition) => LaunchBallToDirection?.Invoke(directionPosition);
    public void OnGameOver() => GameOver?.Invoke();
}