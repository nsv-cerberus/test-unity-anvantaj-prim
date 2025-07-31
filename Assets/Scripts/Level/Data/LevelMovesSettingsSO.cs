using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockData
{
    [SerializeField] private int _number;
    [SerializeField] private int _dropPercentage = 100;

    public int Number => _number;
    public int DropPercentage => _dropPercentage;

    public void ClampPercent(int maxAllowed)
    {
        _dropPercentage = Mathf.Clamp(_dropPercentage, 0, maxAllowed);
    }
}

[System.Serializable]
public struct LevelMoveSettings
{
    [SerializeField] private int _moveCount;
    [SerializeField] private List<BlockData> _blocksData;

    public int MoveCount => _moveCount;
    public List<BlockData> BlocksData => _blocksData;
}

[CreateAssetMenu(menuName = "ScriptableObjects/Level/MovesSettings", fileName = "NewLevelMovesSettings")]
public class LevelMovesSettingsSO : ScriptableObject
{
    [SerializeField] private List<LevelMoveSettings> _moves = new List<LevelMoveSettings>();

#if UNITY_EDITOR
    void OnValidate()
    {
        foreach (var lvl in _moves)
        {
            int sum = 0;

            for (int i = 0; i < lvl.BlocksData.Count; i++)
            {
                int allowed = Mathf.Max(0, 100 - sum);
                lvl.BlocksData[i].ClampPercent(allowed);
                sum += lvl.BlocksData[i].DropPercentage;
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif

    public List<int> BlockNumber(int count)
    {
        var a = new List<int>();
        return a;
    }

    public int GetRandomeBlockNumberByMoveNumber(int currentMoveNumber)
    {
        int currentPercent = 0;
        int randomePercent = RandomUtility.GetRandomInRange(1, 100);

        foreach (var blockData in GetLevelMoveSettingsByMoveNumber(currentMoveNumber).BlocksData)
        {
            if (currentPercent < randomePercent && randomePercent <= currentPercent + blockData.DropPercentage)
            {
                return blockData.Number;
            }
            else
            {                
                currentPercent += blockData.DropPercentage;
            }
        }

        return 0;
    }

    private LevelMoveSettings GetLevelMoveSettingsByMoveNumber(int currentMoveNumber)
    {
        int movesCount = 0;

        foreach (var move in _moves)
        {
            if (movesCount < currentMoveNumber && currentMoveNumber <= movesCount + move.MoveCount)
            {
                return move;
            }
            else
            {
                movesCount += move.MoveCount;
            }
        }

        return _moves[_moves.Count - 1];
    }
}