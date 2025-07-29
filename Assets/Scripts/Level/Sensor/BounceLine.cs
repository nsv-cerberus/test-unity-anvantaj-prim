using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class BounceLine : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Sensor _sensor;
    [SerializeField] private GameObject _dotPrefab;
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private int _dotsCount = 10;
    [SerializeField] private float _maxRayDistance = 1000f;

    private RectTransform _rectTransform;
    private Camera _camera;
    private List<RectTransform> _dots;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _camera = _canvas.renderMode == RenderMode.ScreenSpaceCamera
               ? _canvas.worldCamera
               : Camera.main;

        CreateDots();

        _sensor.DrawBounceLine += Draw;
    }

    private void CreateDots()
    {
        _dots = new List<RectTransform>(_dotsCount);

        for (int i = 0; i < _dotsCount; i++)
        {
            var d = Instantiate(_dotPrefab, _rectTransform).GetComponent<RectTransform>();
            d.gameObject.SetActive(false);
            _dots.Add(d);
        }
    }

    private void Draw(Vector2 screenBallPosition, Vector2 direction)
    {
        if (_camera == null) return;
        var path = CalculatePath(screenBallPosition, direction);
        PlaceDots(path);
    }

    private List<Vector2> CalculatePath(Vector2 screenBallPosition, Vector2 direction)
    {
        var points = new List<Vector2>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform, screenBallPosition, _camera, out Vector2 localStart
        );
        points.Add(localStart);

        float screenRadius = _sensor.Ball.Radius;

        Vector3 screenPoint = new Vector3(0, 0, 0);
        Vector3 screenPointWithRadius = new Vector3(screenRadius, 0, 0);
        float worldRadius = Vector3.Distance(
            _camera.ScreenToWorldPoint(screenPoint),
            _camera.ScreenToWorldPoint(screenPointWithRadius)
        );

        var hits = Physics2D.CircleCastAll(
            _camera.ScreenToWorldPoint(new Vector3(screenBallPosition.x, screenBallPosition.y, 0)),
            worldRadius,
            direction, _maxRayDistance, _collisionMask
        );

        if (hits.Length > 0)
        {
            Vector2 worldHit = hits[0].point;
            Vector2 screenHit = _camera.WorldToScreenPoint(worldHit);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform, screenHit, _camera, out Vector2 localHit
            );

            float radius = _sensor.Ball.Radius;
            Vector2 normalizedDirection = direction.normalized;
            Vector2 adjustedLocalHit = localHit - normalizedDirection * radius;
            points.Add(adjustedLocalHit);

            Vector2 localDir = (localHit - localStart).normalized;

            // нормаль в локал
            Vector3 worldNorm3 = hits[0].normal;
            Vector3 localNorm3 = _rectTransform.InverseTransformDirection(worldNorm3);
            Vector2 localNorm = ((Vector2)localNorm3).normalized;

            Vector2 refl = Vector2.Reflect(localDir, localNorm);

            // конечная точка (по половине дистанции)
            Vector2 localEnd = localHit + refl * (_maxRayDistance * 0.5f);
            points.Add(localEnd);
        }
        else
        {
            // если не попадаем — уходим по локальному направлению
            Vector3 worldDir3 = new Vector3(direction.x, direction.y, 0f);
            Vector3 localDir3 = _rectTransform.InverseTransformDirection(worldDir3);
            Vector2 localDir = ((Vector2)localDir3).normalized;
            points.Add(localStart + localDir * _maxRayDistance);
        }

        return points;
    }

    private void PlaceDots(List<Vector2> path)
    {
        foreach (var d in _dots)
            d.gameObject.SetActive(false);
        if (path.Count < 2) return;

        // рассчитываем общую длину
        float total = 0f;
        for (int i = 0; i < path.Count - 1; i++)
            total += Vector2.Distance(path[i], path[i + 1]);
        float spacing = total / (_dotsCount - 1);

        int idx = 0;
        float passed = 0f;

        // расставляем доты
        for (int i = 0; i < path.Count - 1 && idx < _dotsCount; i++)
        {
            Vector2 A = path[i], B = path[i + 1];
            float segLen = Vector2.Distance(A, B);
            Vector2 dir = (B - A).normalized;

            while (passed + spacing <= segLen && idx < _dotsCount)
            {
                passed += spacing;
                Vector2 pos = A + dir * passed;
                var rt = _dots[idx];
                rt.anchoredPosition = pos;
                rt.gameObject.SetActive(true);
                idx++;
            }
            passed -= segLen;
            if (passed < 0f) passed = 0f;
        }
    }

    private void OnDisable()
    {
        _sensor.DrawBounceLine -= Draw;
    }
}
