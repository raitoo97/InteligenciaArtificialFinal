using System.Collections.Generic;
using UnityEngine;
public class MoveLeaderState : IState
{
    private List<Vector3> _mainPath = new List<Vector3>();
    private Transform _transform;
    private Agent _agent;
    private FSM _fsm;
    private float _nearDistance;
    private Leader _leader;
    public MoveLeaderState(Transform transform, float nearDistnace,Leader leader,List<Vector3> mainPath, Agent agent ,FSM fsm)
    {
        _transform = transform;
        _nearDistance = nearDistnace;
        _mainPath = mainPath;
        _agent = agent;
        _leader = leader;
        _fsm = fsm;
    }
    public void OnEnter()
    {
    }
    public void OnUpdate()
    {
        var allBoids = BoidManager.instance.GetBoids;
        List<Boid> enemyBoids;
        if (_leader.IsVioletLeader)
        {
            enemyBoids = allBoids.FindAll(b => b.typeBoid == TypeBoid.BlueTeam);
        }
        else
        {
            enemyBoids = allBoids.FindAll(b => b.typeBoid == TypeBoid.VioletTeam);
        }
        foreach (var boid in enemyBoids)
        {
            if (FOV.InFieldOfView(boid.transform, _leader.transform,_leader.ViewRadius, _leader.ViewAngle))
            {
                _fsm.ChangeState(FSM.State.Attack);
                return;
            }
        }
        if (_mainPath.Count <= 0)
        {
            _fsm.ChangeState(FSM.State.Idle);
            return;
        }
        MoveAlongPath();
    }
    private void MoveAlongPath()
    {
        if (_mainPath.Count == 0) return;
        var target = _mainPath[0];
        Vector3 dir = target - _transform.position;
        _leader.RotateTo(dir);
        _agent.ApplyArrive(target);
        if (Vector3.Distance(_transform.position, target) < _nearDistance)
            _mainPath.RemoveAt(0);
    }
    public void OnExit()
    {
    }
}
