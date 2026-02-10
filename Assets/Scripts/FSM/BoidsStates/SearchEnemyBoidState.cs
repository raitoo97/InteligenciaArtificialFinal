using System.Collections.Generic;
using UnityEngine;
public class SearchEnemyBoidState : IState
{
    private Boid _boid;
    private Agent _agent;
    private FSM _fsm;
    private Transform _target;
    private float _searchTimer;
    private float _searchInterval = 0.5f;
    public SearchEnemyBoidState(Boid boid, Agent agent, FSM fsm)
    {
        _boid = boid;
        _agent = agent;
        _fsm = fsm;
    }
    public void OnEnter()
    {
        _target = ChooseRandomEnemy();
        _searchTimer = _searchInterval;
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
        {
            _searchTimer -= Time.deltaTime;
            if (_searchTimer <= 0f)
            {
                _searchTimer = _searchInterval;
                _target = ChooseRandomEnemy();
            }
            return;
        }
        var visibleEnemy = _boid.GetClosestVisibleEnemy();
        if (visibleEnemy != null)
        {
            _boid.ClearPath();
            _fsm.ChangeState(FSM.State.Attack);
            return;
        }
        if (_boid.GetPath.Count > 0)
        {
            _boid.MoveAlongPath();
            return;
        }
        if (_target != null)
            _boid.CalculatePathToTarget(_target.position);
    }
    private Transform ChooseRandomEnemy()
    {
        List<Boid> enemies = new List<Boid>();
        foreach (var boid in BoidManager.instance.GetBoids)
        {
            if (boid == null) continue;
            if (boid.typeBoid != _boid.typeBoid)
                enemies.Add(boid);
        }
        if (enemies.Count > 0)
        {
            int index = Random.Range(0, enemies.Count);
            return enemies[index].transform;
        }
        var enemyLeader = LeaderManager.instance.GetLeader(_boid.Leader);
        if (enemyLeader != null)
            return enemyLeader.transform;
        return null;
    }
}
