using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BlockSettings
{
    [SerializeField] private int _number;
    [SerializeField] private Color _color;

    public int Number => _number;
    public Color Color => _color;
}

[CreateAssetMenu(menuName = "ScriptableObjects/Level/BlocksSettings", fileName = "NewBlocksSettings")]
public class BlocksSettingsSO : ScriptableObject
{
    [SerializeField] private List<BlockSettings> _blocksSettings = new List<BlockSettings>();

    public Color GetColorByNumber(int number)
    {
        foreach (var block in _blocksSettings)
        {
            if (block.Number == number)
            {
                return block.Color;
            }
        }

        return Color.white;
    }
}