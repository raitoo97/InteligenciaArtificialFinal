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
    private Vector3 _lastKnowPosition;
    private float _distanceTreshold = 5f;
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
        if (_target != null)
            _lastKnowPosition = _target.position;
    }
    public void OnExit()
    {
        _boid.ClearPath();
    }
    public void OnUpdate()
    {
        if(_boid.Life.GetLife <= _boid.MinLifeToRetreat)
        {
            _boid.ClearPath();
            _fsm.ChangeState(FSM.State.Retreat);
            return;
        }
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
        if (_target != null)
        {
            float distance = Vector3.Distance(_target.position, _lastKnowPosition);
            if (_boid.GetPath.Count == 0 || distance > _distanceTreshold)
            {
                var isOnSight = LineOfSight.IsOnSight(_boid.transform.position, _target.position);
                if (isOnSight)
                    _boid.GoDirectToTarget(_target.position);
                else
                    _boid.CalculatePathToTarget(_target.position);
                _lastKnowPosition = _target.position;
            }
        }
        if (_boid.GetPath.Count > 0)
        {
            _boid.MoveAlongPath();
        }
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