using System.Collections.Generic;
using UnityEngine;
public class SearchEnemyBoidState : IState
{
    private Boid _boid;
    private Agent _agent;
    private FSM _fsm;
    private Transform _target;
    public SearchEnemyBoidState(Boid boid, Agent agent, FSM fsm)
    {
        _boid = boid;
        _agent = agent;
        _fsm = fsm;
    }
    public void OnEnter()
    {
        _target = ChooseRandomEnemy();
        _boid.ClearPath();
        _agent.ChangeMove(true);
    }
    public void OnExit()
    {
        _boid.ClearPath();
    }
    public void OnUpdate()
    {
        if (_target == null)
            return;
        Vector3 dir = _target.position - _boid.transform.position;
        if (_boid.GetPath.Count > 0)
        {
            _boid.MoveAlongPath();
            return;
        }
        if (LineOfSight.IsOnSight(_boid.transform.position, _target.position))
        {
            _fsm.ChangeState(FSM.State.Attack);
            return;
        }
        _boid.CalculatePathToTarget(_target.position);
    }
    private Transform ChooseRandomEnemy()
    {
        List<Boid> enemies = new List<Boid>();
        foreach (var boid in BoidManager.instance.GetBoids)
        {
            if (boid.typeBoid != _boid.typeBoid)
                enemies.Add(boid);
        }
        if (enemies.Count == 0)
            return null;
        int index = Random.Range(0, enemies.Count);
        return enemies[index].transform;
    }
}
