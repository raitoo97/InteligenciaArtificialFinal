using UnityEngine;
public class MoveBoidState : IState
{
    private Leader _leader;
    private Boid _boid;
    private FSM _fsm;
    public MoveBoidState(Boid boid, Leader leader,FSM fsm)
    {
        _leader = leader;
        _boid = boid;
        _fsm = fsm;
    }
    public void OnEnter()
    {
        _boid.ResetAlert();
    }
    public void OnUpdate()
    {
        if (_boid.DetectEnemy())
        {
            _fsm.ChangeState(FSM.State.Attack);
            return;
        }
        var distance = Vector3.Distance(_boid.transform.position, _leader.transform.position);
        if (distance < _boid._distanceToLeader)
        {
            _fsm.ChangeState(FSM.State.Idle);
            return;
        }
        _boid.ApplySeparation();
        if (_boid.GetPath.Count > 0)
        {
            _boid.MoveAlongPath();
            return;
        }
        if (LineOfSight.IsOnSight(_boid.transform.position, _leader.transform.position))
        {
            _boid.GoDirectToTarget(_leader.transform.position);
        }
        else
        {
            _boid.CalculatePathToTarget(_leader.transform.position);
        }
    }  
    public void OnExit()
    {
    }
}