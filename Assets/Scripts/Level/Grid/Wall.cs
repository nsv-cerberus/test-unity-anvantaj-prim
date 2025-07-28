using UnityEngine;

[RequireComponent(typeof(RectTransform), typeof(BoxCollider2D))]
public class Wall : MonoBehaviour
{
    private RectTransform _rectTransform;
    private BoxCollider2D _boxCollider;

    private void OnValidate()
    {
        UpdateCollider();
    }

    private void Awake()
    {
        UpdateCollider();
    }

    private void UpdateCollider()
    {
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        if (_boxCollider == null) _boxCollider = GetComponent<BoxCollider2D>();

        Vector2 size = _rectTransform.rect.size;
        Vector2 center = _rectTransform.rect.center;

        _boxCollider.size = size;
        _boxCollider.offset = center;
    }
}