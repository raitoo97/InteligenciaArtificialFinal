using System.Collections.Generic;
using UnityEngine;
public class Boid : Agent , IFlockingSeparation
{
    private FSM _fsm;
    [Header("BoidConfig")]
    [SerializeField]public List<Boid> _neigboards = new List<Boid>();
    public float weightSeparation;
    [Range(0f, 2f)]public float radiusSeparation;
    public Leader leader;
    private void OnEnable()
    {
        _fsm = new FSM();
    }
    protected override void Start()
    {
        base.Start();
        _fsm.ChangeState(FSM.State.Idle);
    }
    protected override void Update()
    {
        FlockingAndArrive();
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
    public void FlockingAndArrive()
    {
        ApplyArrive(leader.transform.position);
        AddForce(Separation(_neigboards, radiusSeparation) * weightSeparation);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, radiusSeparation);
    }
}
