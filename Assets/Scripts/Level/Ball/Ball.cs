using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    [SerializeField] private float _velocity = 1f;

    [Inject] private LevelGameplayManager _levelGameplayManager;

    private CircleCollider2D _circleCollider;
    private Rigidbody2D _rigidbody;
    private Coroutine _velocityCoroutine;
    private Vector2 _direction;

    private void Awake()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _levelGameplayManager.LaunchBallToDirection += LaunchToDirection;
        _levelGameplayManager.GameOver += ResetVelocity;
    }

    private void LaunchToDirection(Vector2 direction)
    {
        _direction = direction;
        SetVelocity();
        _levelGameplayManager.SetBallMoveState(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.isTrigger)
        {
            if (collision.collider.CompareTag("BallStopper"))
            {   
                if (_rigidbody.linearVelocity != Vector2.zero)
                {
                    ResetVelocity();
                }

                return;
            }

            if (collision.collider.CompareTag("Block"))
            {
                Vector2 collisionVector = collision.contacts[0].point - (Vector2)transform.position;
                float angle = Mathf.Atan2(collisionVector.y, collisionVector.x) * Mathf.Rad2Deg;
                Block block = collision.collider.GetComponent<Block>();

                if (angle > 45 && angle < 135)
                {
                    block.TryMoveBlockToDirection(BlockMoveDirection.Up);
                }
                else if (angle > -135 && angle < -45)
                {
                    block.TryMoveBlockToDirection(BlockMoveDirection.Down);
                }
                else if (angle > -45 && angle < 45)
                {
                    block.TryMoveBlockToDirection(BlockMoveDirection.Right);
                }
                else
                {
                    block.TryMoveBlockToDirection(BlockMoveDirection.Left);
                }
            }

            foreach (var contact in collision.contacts)
            {
                _direction = Vector2.Reflect(_direction, contact.normal).normalized;
                break;
            }
        }
    }

    public float Radius => _circleCollider.radius;

    private void SetVelocity() => _rigidbody.linearVelocity = _direction * _velocity;
    private void ResetVelocity() 
    {
        _rigidbody.linearVelocity = Vector2.zero;
        _levelGameplayManager.SetBallMoveState(false);
    }

    private void OnDestroy()
    {
        _levelGameplayManager.LaunchBallToDirection -= LaunchToDirection;
        _levelGameplayManager.GameOver -= ResetVelocity;
    }
}