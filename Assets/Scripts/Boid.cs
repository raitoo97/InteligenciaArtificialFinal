using System.Collections.Generic;
using UnityEngine;
public enum TypeBoid
{
    BlueTeam,
    VioletTeam
}
public class Boid : Agent , IFlockingSeparation
{
    private FSM _fsm;
    [Header("BoidConfig")]
    [SerializeField]public List<Boid> _neigboards = new List<Boid>();
    public TypeBoid typeBoid;
    public float weightSeparation;
    public float leaderSeparationWeight;
    [SerializeField]private Leader _leaderRef;
    [Range(0f, 5f)] public float radiusSeparation;
    [Range(0f, 4f)] public  float _distanceToLeader;
    [SerializeField] private float _viewRadius;
    [SerializeField] private float _viewAngle;
    private void OnEnable()
    {
        _fsm = new FSM();
        _fsm.AddState(FSM.State.Move, new MoveBoidState(this,_leaderRef,this,_fsm));
        _fsm.AddState(FSM.State.Idle, new IdleBoidState(this,_leaderRef,this,_fsm));
    }
    protected override void Start()
    {
        base.Start();
        _fsm.ChangeState(FSM.State.Idle);
    }
    protected override void Update()
    {
        _fsm?.OnUpdate();
        base.Update();
    }
    public Vector3 Separation(List<Boid> boids, float range)
    {
        var desired = Vector3.zero;
        foreach(var boid in boids)
        {
            var direction = this.transform.position - boid.transform.position;
            var distance = direction.magnitude;
            if (distance > range || boid == this) continue;
            desired += direction;
        }
        if (desired == Vector3.zero)
            return Vector3.zero;
        desired = desired.normalized * _maxSpeed;
        var steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce);
        return steering;
    }
    public Vector3 SeparationFromLeader()
    {
        if (_leaderRef == null) return Vector3.zero;
        var direction = this.transform.position - _leaderRef.transform.position;
        var distance = direction.magnitude;
        if (distance > radiusSeparation) return Vector3.zero;
        var desired = direction.normalized * _maxSpeed;
        var steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce);
        return steering;
    }
    public void ApplySeparation()
    {
        AddForce(Separation(_neigboards, radiusSeparation) * weightSeparation);
        AddForce(SeparationFromLeader() * leaderSeparationWeight);
    }
    public void RotateTo(Vector3 dir)
    {
        if (dir != Vector3.zero)
        {
            dir.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, radiusSeparation);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, _distanceToLeader);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);
        Vector3 rightDir = Quaternion.Euler(0, _viewAngle * 0.5f, 0) * transform.forward;
        Vector3 leftDir = Quaternion.Euler(0, -_viewAngle * 0.5f, 0) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + rightDir * _viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + leftDir * _viewRadius);
    }
}
