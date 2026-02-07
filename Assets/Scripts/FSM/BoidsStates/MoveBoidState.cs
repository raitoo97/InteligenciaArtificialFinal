using System.Collections.Generic;
using UnityEngine;
public class MoveBoidState : IState
{
    private Leader _leader;
    private Agent _agent;
    private Boid _boid;
    private FSM _fsm;
    private List<Vector3> _pathToLeader = new List<Vector3>();
    public MoveBoidState(Boid boid, Leader leader,Agent agent,FSM fsm)
    {
        _leader = leader;
        _agent = agent;
        _boid = boid;
        _fsm = fsm;
    }
    public void OnEnter()
    {
    }
    public void OnUpdate()
    {
        var distance = Vector3.Distance(_boid.transform.position, _leader.transform.position);
        if (distance < _boid._distanceToLeader)
        {
            _fsm.ChangeState(FSM.State.Idle);
            return;
        }
        _boid.ApplySeparation();
        if (_pathToLeader.Count > 0)
        {
            MoveAlongPath();
            return;
        }
        if (LineOfSight.IsOnSight(_boid.transform.position, _leader.transform.position))
        {
            Debug.Log("Go direct to leader");
            GoDirectToLeader(_leader.transform.position);
        }
        else
        {
            Debug.Log("Calculate path to leader");
            CalculatePathToLeader(_leader.transform.position);
        }

    }
    private void MoveAlongPath()
    {
        if (_pathToLeader.Count == 0) return;
        var target = _pathToLeader[0];
        _agent.ApplyArrive(target);
        if (Vector3.Distance(_boid.transform.position, target) < 1.5f)
            _pathToLeader.RemoveAt(0);
    }
    private void GoDirectToLeader(Vector3 target)
    {
        _pathToLeader.Clear();
        _pathToLeader.Add(target);
    }
    public void CalculatePathToLeader(Vector3 target)
    {
        _pathToLeader.Clear();
        var path = PathFinding.CalculateTheta(_boid.transform.position, target);
        _pathToLeader.AddRange(path);
    }   
    public void OnExit()
    {
    }
}
