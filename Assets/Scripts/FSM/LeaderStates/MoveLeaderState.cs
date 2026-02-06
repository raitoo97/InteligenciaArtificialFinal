using System.Collections.Generic;
using UnityEngine;
public class MoveLeaderState : IState
{
    private List<Vector3> _mainPath = new List<Vector3>();
    private Transform _transform;
    private Agent _agent;
    private FSM _fsm;
    private float _nearDistance;
    public MoveLeaderState(Transform transform, float nearDistnace,List<Vector3> mainPath, Agent agent ,FSM fsm)
    {
        _transform = transform;
        _nearDistance = nearDistnace;
        _mainPath = mainPath;
        _agent = agent;
        _fsm = fsm;
    }
    public void OnEnter()
    {
        Debug.Log("Enter Move Leader State");
    }
    public void OnUpdate()
    {
        if (_mainPath.Count <= 0)
            _fsm.ChangeState(FSM.State.Idle);
        MoveAlongPath();
    }
    private void MoveAlongPath()
    {
        if (_mainPath.Count == 0) return;
        var target = _mainPath[0];
        _agent.ApplyArrive(target);
        if (Vector3.Distance(_transform.position, target) < _nearDistance)
            _mainPath.RemoveAt(0);
    }
    public void OnExit()
    {
        Debug.Log("Exit Move Leader State");
    }
}
