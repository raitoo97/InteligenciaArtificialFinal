using UnityEngine;
public abstract class Agent : MonoBehaviour
{
    protected Vector3 _velocity;
    [Header("Agent Propieties")]
    [SerializeField][Range(0, 8)]protected float _maxSpeed;
    [SerializeField][Range(0,1)]protected float _maxForce;
    [SerializeField][Range(0,3)]protected float _obstacleAvoidanceWeight;
    [SerializeField]protected float radiusArrive;
    [SerializeField]protected bool _canMove;
    protected virtual void Start()
    {
        _velocity = Vector3.zero;
        _canMove = true;
    }
    protected virtual void Update()
    {
        if (!_canMove) return;
        transform.position += _velocity * Time.deltaTime;
    }
    public void AddForce (Vector3 dir)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + dir, _maxSpeed);
        _velocity.y = 0;
    }
    protected Vector3 Seek (Vector3 target)
    {
        var desired = (target - transform.position).normalized * _maxSpeed;
        var steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce);
        return steering;
    }
    protected Vector3 Arrive (Vector3 target)
    {
        var desired = target - transform.position;
        var distance = desired.magnitude;
        if(distance > radiusArrive)
            return Seek(target);
        desired = desired.normalized * _maxSpeed * (distance / radiusArrive);
        var steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce);
        return steering;
    }
    protected Vector3 Flee(Vector3 target)
    {
        var desired = (transform.position - target).normalized * _maxSpeed;
        var steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce);
        return steering;
    }
    public void ApplyArrive(Vector3 target)
    {
        AddForce(Arrive(target));
    }
    public void ApplySeek(Vector3 target)
    {
        AddForce(Seek(target));
    }
    public void ApplyFlee(Vector3 target)
    {
        AddForce(Flee(target));
    }
    public void ChangeMove(bool canMove)
    {
        _canMove = canMove;
        if (!canMove)
        {
            _velocity = Vector3.zero;
        }
    }
    public Vector3 Velocity { get => _velocity; }
    public float MaxSpeed { get => _maxSpeed; }
    public float MaxForce { get => _maxForce; }
    public float ObstacleAvoidanceWeight { get => _obstacleAvoidanceWeight; }
}
