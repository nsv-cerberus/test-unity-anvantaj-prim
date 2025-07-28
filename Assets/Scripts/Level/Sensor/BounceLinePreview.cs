using System.Collections.Generic;
using UnityEngine;

public class BounceLinePreview : MonoBehaviour
{
    [SerializeField] private GameObject _dotPrefab;
    [SerializeField] private Transform _dotsContainer;
    [SerializeField] private LayerMask _bounceMask;
    [SerializeField] private int _dotCount = 10;

    private List<GameObject> _dots = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < _dotCount; i++)
        {
            GameObject dot = Instantiate(_dotPrefab, _dotsContainer);
            dot.SetActive(false);
            _dots.Add(dot);
        }
    }

    public void ShowPreview(Vector2 startPoint, Vector2 direction)
    {
        List<Vector2> pathPoints = CalculateBouncePath(startPoint, direction);
        PlaceDots(pathPoints);
    }

    private List<Vector2> CalculateBouncePath(Vector2 startPoint, Vector2 direction)
    {
        List<Vector2> points = new List<Vector2>();
        RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, 100f, _bounceMask);

        if (hit.collider != null)
        {
            Vector2 bouncePoint = hit.point;
            Vector2 reflectedDir = Vector2.Reflect(direction, hit.normal);

            points.Add(startPoint);
            points.Add(bouncePoint);
            points.Add(bouncePoint + reflectedDir.normalized * 5f);
        }
        else
        {
            points.Add(startPoint);
            points.Add(startPoint + direction.normalized * 10f);
        }

        return points;
    }

    private void PlaceDots(List<Vector2> pathPoints)
    {
        foreach (var dot in _dots)
            dot.SetActive(false);

        float totalLength = 0f;
        for (int i = 0; i < pathPoints.Count - 1; i++)
            totalLength += Vector2.Distance(pathPoints[i], pathPoints[i + 1]);

        float spacing = totalLength / (_dotCount - 1);
        int currentDot = 0;
        float distPassed = 0f;

        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector2 from = pathPoints[i];
            Vector2 to = pathPoints[i + 1];
            float segmentLength = Vector2.Distance(from, to);
            Vector2 direction = (to - from).normalized;

            while (distPassed + spacing <= segmentLength && currentDot < _dotCount)
            {
                distPassed += spacing;
                Vector2 dotPos = from + direction * distPassed;

                GameObject dot = _dots[currentDot];
                dot.transform.position = dotPos;
                dot.SetActive(true);
                currentDot++;
            }

            distPassed = distPassed - segmentLength;
            if (distPassed < 0f) distPassed = 0f;
        }
    }
}