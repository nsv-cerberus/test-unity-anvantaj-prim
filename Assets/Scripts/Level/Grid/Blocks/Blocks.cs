using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Blocks : MonoBehaviour
{
    private Grid _grid = new Grid();
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private BlocksSettingsSO _blocksSettings;
    [SerializeField] private LevelMovesSettingsSO _levelMovesSettings;

    [Inject] private DiContainer _diContainer;
    [Inject] private LevelGameplayManager _levelGameplayManager;

    private BlocksPool _blocksPool = new BlocksPool();
    private const int _minBlocksInRow = 1;
    private const int _maxBlocksInRow = 2;

    public Grid Grid => _grid;
    public BlocksPool BlocksPool => _blocksPool;

    private void Awake()
    {
        var blockPrefabRectTransform = _blockPrefab.GetComponent<RectTransform>();
        var width = blockPrefabRectTransform.rect.width;
        var height = blockPrefabRectTransform.rect.height;
        _grid.DefineCellsPositions(width, height);

        _levelGameplayManager.CreateNewRow += CreateNewRow;
    }

    private void CreateNewRow()
    {
        CreateNewBlocks(GetRandomeColumnsIndexes());
        _levelGameplayManager.OnMoveBlocksDown();
    }

    private List<int> GetRandomeColumnsIndexes()
    {
        int blocksInLineCount = RandomUtility.GetRandomInRange(_minBlocksInRow, _maxBlocksInRow);
        var randomColumnsIndexes = GetRandomColumnsIndexesForSpawnBlocks(blocksInLineCount);

        Debug.Log($"Random Columns Indexes: {string.Join(", ", randomColumnsIndexes)}");

        randomColumnsIndexes.Sort();
        return randomColumnsIndexes;
    }

    private List<int> GetRandomColumnsIndexesForSpawnBlocks(int count)
    {
        List<int> indexesColumns = new List<int>();

        for (int i = 0; i < _grid.Cells.GetLength(1); i++)
        {
            indexesColumns.Add(i);
        }

        ShuffleList(indexesColumns);
        return indexesColumns.GetRange(0, count);
    }

    private void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = RandomUtility.GetRandomInRange(0, i);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void CreateNewBlocks(List<int> columnsIndexes)
    {
        foreach (var columnIndex in columnsIndexes)
        {
            Block newBlock = _blocksPool.Get();

            if (newBlock == null)
            {
                newBlock = Instantiate(_blockPrefab, transform);
                _diContainer.Inject(newBlock);
            }

            int blockNumber = _levelMovesSettings.GetRandomeBlockNumberByMoveNumber(_levelGameplayManager.Move);
            newBlock.Initialize(blockNumber, _blocksSettings.GetColorByNumber(blockNumber), columnIndex, this);
        }
    }

    private bool CheckMergeBlocks(Block block1, Block block2) => (block1.Number == block2.Number) ? true : false;
}