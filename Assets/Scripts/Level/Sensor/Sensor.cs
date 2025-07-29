using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Sensor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public event Action<Vector2, Vector2> DrawBounceLine;
    public event Action ResetBounceLine;

    [SerializeField] private Ball _ball;

    [Inject] private LevelGameplayManager _levelGameplayManager;
    
    private bool _isWorking = false;

    public Ball Ball => _ball;

    private void Awake()
    {
        _levelGameplayManager.ActivateSensor += ActivateSensor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_isWorking) return;

        Vector2 touchPosition = eventData.position;
        Vector2 screenBallPosition = Camera.main.WorldToScreenPoint(_ball.transform.position);
        Vector2 direction = (touchPosition - screenBallPosition).normalized;

        OnDrawBounceLine(screenBallPosition, direction);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isWorking) return;

        Vector2 touchPosition = eventData.position;
        Vector2 screenBallPosition = Camera.main.WorldToScreenPoint(_ball.transform.position);
        Vector2 direction = (touchPosition - screenBallPosition).normalized;

        OnDrawBounceLine(screenBallPosition, direction);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isWorking) return;

        Vector2 touchPosition = eventData.position;
        Vector2 screenBallPosition = Camera.main.WorldToScreenPoint(_ball.transform.position);
        Vector2 direction = (touchPosition - screenBallPosition).normalized;

        _levelGameplayManager.OnLaunchBallToDirection(direction);
        DeactivateSensor();
    }

    private void ActivateSensor() => _isWorking = true;
    private void DeactivateSensor() => _isWorking = false;

    private void OnDrawBounceLine(Vector2 screenStartPosition, Vector2 direction) => DrawBounceLine?.Invoke(screenStartPosition, direction);
    private void OnResetBounceLine() => ResetBounceLine?.Invoke();

    public void OnDestroy()
    {
        _levelGameplayManager.ActivateSensor -= ActivateSensor;
    }   
}