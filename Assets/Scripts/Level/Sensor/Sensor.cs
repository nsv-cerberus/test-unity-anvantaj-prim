using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

[RequireComponent(typeof(BounceLinePreview))]
public class Sensor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Ball _ball;

    [Inject] private LevelGameplayManager _levelGameplayManager;

    private BounceLinePreview _bounceLinePreview;
    private bool _isWorking = false;

    private void Awake()
    {
        _bounceLinePreview = GetComponent<BounceLinePreview>();

        _levelGameplayManager.ActivateSensor += ActivateSensor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_isWorking) return;

        Vector3 startPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        startPosition.z = 0;

        Vector3 ballPosition = Camera.main.ScreenToWorldPoint(_ball.transform.position);
        Vector2 direction = (startPosition - ballPosition).normalized;

        _bounceLinePreview.ShowPreview(_ball.transform.position, direction);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isWorking) return;

        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        currentPosition.z = 0;

        Vector3 ballPosition = Camera.main.ScreenToWorldPoint(_ball.transform.position);
        Vector2 direction = (currentPosition - ballPosition).normalized;

        _bounceLinePreview.ShowPreview(_ball.transform.position, direction);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isWorking) return;

        Vector3 endPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        _levelGameplayManager.OnLaunchBallToDirection(new Vector2(endPosition.x, endPosition.y));
        DeactivateSensor();

        // _bounceLinePreview.Clear(); -- нужно сделать очистку отдельно!
    }

    private void ActivateSensor() => _isWorking = true;
    private void DeactivateSensor() => _isWorking = false;

    public void OnDestroy()
    {
        _levelGameplayManager.ActivateSensor -= ActivateSensor;
    }   
}