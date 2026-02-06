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
    [Range(0f, 2f)]public float radiusSeparation;
    [SerializeField]private Leader _leaderRef;
    public  float _distanceToLeader;
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
        ApplySeparation();
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
    public void ApplySeparation()
    {
        AddForce(Separation(_neigboards, radiusSeparation) * weightSeparation);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, radiusSeparation);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, _distanceToLeader);
    }
}
