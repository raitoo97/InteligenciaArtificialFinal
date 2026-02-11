using UnityEngine;
public class RetreatBoidState : IState
{
    private Boid _boid;
    private FSM _fsm;
    private Agent _agent;
    private Transform _safeSpot;
    public RetreatBoidState(Boid boid,Agent agent , FSM fsm)
    {
        _boid = boid;
        _fsm = fsm;
        _agent = agent;
    }

    public void OnEnter()
    {
        _boid.ClearPath();
        _safeSpot = GetClosestSafeSpot();
        _agent.ChangeMove(true);
        if (_safeSpot != null)
        {
            if (LineOfSight.IsOnSight(_boid.transform.position, _safeSpot.position))
                _boid.GoDirectToTarget(_safeSpot.position);
            else
                _boid.CalculatePathToTarget(_safeSpot.position);
        }
    }
    public void OnExit()
    {
        _boid.ClearPath();
    }
    public void OnUpdate()
    {
        if (_boid.GetPath.Count > 0)
        {
            _boid.MoveAlongPath();
            _boid.ApplySeparation();
            return;
        }
        _fsm.ChangeState(FSM.State.Attack);
    }
    private Transform GetClosestSafeSpot()
    {
        _safeSpot = _boid.typeBoid == TypeBoid.BlueTeam ? _boid._blueSecurePlace : _boid._violetSecurePlace;
        return _safeSpot;
    }
}
