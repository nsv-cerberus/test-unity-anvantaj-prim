using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class BounceLine : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameObject _dotPrefab;
    [SerializeField] private int _dotsCount = 10;

    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _maxRayDistance = 1000f;

    [SerializeField] private Sensor _sensor;

    private RectTransform _rt;
    private Camera _cam;
    private float _planeDist;
    private List<RectTransform> _dots;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _cam = _canvas.renderMode == RenderMode.ScreenSpaceCamera
               ? _canvas.worldCamera
               : Camera.main;
        _planeDist = _canvas.planeDistance;

        // создаём пул точек
        _dots = new List<RectTransform>(_dotsCount);
        for (int i = 0; i < _dotsCount; i++)
        {
            var d = Instantiate(_dotPrefab, _rt).GetComponent<RectTransform>();
            d.gameObject.SetActive(false);
            _dots.Add(d);
        }

        _sensor.DrawBounceLine += Draw;
    }

    private void OnDisable()
    {
        _sensor.DrawBounceLine -= Draw;
    }

    private void Draw(Vector2 screenStart, Vector2 screenDir)
    {
        if (_cam == null) return;
        var path = CalculatePath(screenStart, screenDir);
        PlaceDots(path);
    }

    private List<Vector2> CalculatePath(Vector2 screenStart, Vector2 screenDir)
    {
        var pts = new List<Vector2>();

        // 1) стартовая точка в локале RectTransform
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rt, screenStart, _cam, out Vector2 localStart
        );
        pts.Add(localStart);

        // 2) подготавливаем мир‑луч
        Vector3 sp0 = new Vector3(screenStart.x, screenStart.y, _planeDist);
        Vector3 sp1 = new Vector3(
            screenStart.x + screenDir.x,
            screenStart.y + screenDir.y,
            _planeDist
        );
        Vector2 wStart = _cam.ScreenToWorldPoint(sp0);
        Vector2 wEnd = _cam.ScreenToWorldPoint(sp1);
        Vector2 wDir = (wEnd - wStart).normalized;

        // 3) кастим все попадания
        var hits = Physics2D.RaycastAll(
            wStart, wDir, _maxRayDistance, _collisionMask
        );

        if (hits.Length > 0)
        {
            // 4) точка удара
            Vector2 worldHit = hits[0].point;
            Vector2 screenHit = _cam.WorldToScreenPoint(worldHit);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rt, screenHit, _cam, out Vector2 localHit
            );
            pts.Add(localHit);

            // 5) отражаем в локале
            Vector2 localDir = (localHit - localStart).normalized;

            // нормаль в локал
            Vector3 worldNorm3 = hits[0].normal;
            Vector3 localNorm3 = _rt.InverseTransformDirection(worldNorm3);
            Vector2 localNorm = ((Vector2)localNorm3).normalized;

            Vector2 refl = Vector2.Reflect(localDir, localNorm);

            // 6) конечная точка (по половине дистанции)
            Vector2 localEnd = localHit + refl * (_maxRayDistance * 0.5f);
            pts.Add(localEnd);
        }
        else
        {
            // если не попали — уходим по локальному направлению
            Vector3 worldDir3 = new Vector3(wDir.x, wDir.y, 0f);
            Vector3 localDir3 = _rt.InverseTransformDirection(worldDir3);
            Vector2 localDir = ((Vector2)localDir3).normalized;
            pts.Add(localStart + localDir * _maxRayDistance);
        }

        return pts;
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
}
