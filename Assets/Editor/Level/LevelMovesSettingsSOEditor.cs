using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(LevelMovesSettingsSO))]
public class LevelMovesSettingsSOEditor : Editor
{
    private ReorderableList _list;
    private SerializedProperty _movesProp;

    private void OnEnable()
    {
        _movesProp = serializedObject.FindProperty("_moves");
        _list = new ReorderableList(serializedObject, _movesProp, true, true, true, true);

        _list.drawHeaderCallback = rect =>
            EditorGUI.LabelField(rect, "Moves (MoveCount + computed Range)");

        _list.drawElementCallback = DrawElement;
        _list.elementHeightCallback = CalculateElementHeight;
    }

    private void DrawElement(Rect rect, int index, bool active, bool focused)
    {
        var elem = _movesProp.GetArrayElementAtIndex(index);
        var moveCountProp = elem.FindPropertyRelative("_moveCount");
        var blocksDataProp = elem.FindPropertyRelative("_blocksData");

        int start = 1;
        for (int i = 0; i < index; i++)
            start += _movesProp.GetArrayElementAtIndex(i)
                              .FindPropertyRelative("_moveCount")
                              .intValue;
        int end = start + moveCountProp.intValue - 1;

        var lineY = rect.y + 2;
        EditorGUI.LabelField(
            new Rect(rect.x, lineY, rect.width, EditorGUIUtility.singleLineHeight),
            $"Range: {start} – {end}"
        );

        lineY += EditorGUIUtility.singleLineHeight + 4;
        EditorGUI.PropertyField(
            new Rect(rect.x, lineY, rect.width, EditorGUIUtility.singleLineHeight),
            moveCountProp, new GUIContent("MoveCount")
        );

        lineY += EditorGUIUtility.singleLineHeight + 4;
        var blocksRect = new Rect(rect.x, lineY, rect.width,
            EditorGUI.GetPropertyHeight(blocksDataProp, true)
        );
        EditorGUI.PropertyField(blocksRect, blocksDataProp, new GUIContent("Blocks Numbers"), true);
    }

    private float CalculateElementHeight(int index)
    {
        var elem = _movesProp.GetArrayElementAtIndex(index);
        var blocksDataProp = elem.FindPropertyRelative("_blocksData");

        float height = 2 * EditorGUIUtility.singleLineHeight + 8;
        height += EditorGUI.GetPropertyHeight(blocksDataProp, true);
        return height;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}