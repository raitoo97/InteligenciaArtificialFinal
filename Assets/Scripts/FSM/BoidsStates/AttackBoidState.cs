using UnityEngine;
public class AttackBoidState : IState
{
    private Boid _boid;
    private Agent _agent;
    private FSM _fsm;
    private Transform _target;
    private Transform _gunSight;
    private float _attackRange = 10f;
    private float _shootRange = 3f;
    private float _maxCooldown;
    private float _currentCooldown;
    private bool _hasNeighbors;
    private bool _leaderTooClose;
    private bool _hasEnemyNearby;
    private bool _isStopped;
    public AttackBoidState(Transform gunSight,Boid boid,Agent agent,FSM fsm)
    {
        _agent = agent;
        _boid = boid;
        _fsm = fsm;
        _gunSight = gunSight;
    }
    public void OnEnter()
    {
        _target = FindTarget();
        _maxCooldown = 3;
        _isStopped = false;
        _boid.ClearPath();
        Debug.Log("Boid enter attack");
    }
    public void OnExit()
    {
        _agent.ChangeMove(true);
        _boid.ClearPath();
    }
    public void OnUpdate()
    {
        if (_target == null)
        {
            _fsm.ChangeState(FSM.State.SearchEnemy);
            return;
        }
        Vector3 dir = _target.position - _boid.transform.position;
        var dist = dir.magnitude;
        if (_boid.GetPath.Count > 0)
        {
            _boid.MoveAlongPath();
            return;
        }
        bool hasLOS = LineOfSight.IsOnSight(_boid.transform.position,_target.position);
        if (!hasLOS)
        {
            _boid.CalculatePathToTarget(_target.position);
            return;
        }
        if (dist > _attackRange)
        {
            _boid.GoDirectToTarget(_target.position);
            return;
        }
        _leaderTooClose = _boid.HasLeaderTooClose();
        _boid.CheckHasNeighbors(ref _hasNeighbors);
        _boid.CheckHasEnemyNeighbors(ref _hasEnemyNearby, _shootRange);
        bool shouldMove = _hasNeighbors || _leaderTooClose || _hasEnemyNearby;
        if (shouldMove && _isStopped)
        {
            _agent.ChangeMove(true);
            _isStopped = false;
        }
        else if (!shouldMove && !_isStopped)
        {
            _agent.ChangeMove(false);
            _isStopped = true;
        }
        if (shouldMove)
        {
            _boid.ApplySeparation();
            if (_hasEnemyNearby)
                _boid.ApplyEnemySeparation(_attackRange);
        }
        if (dir.sqrMagnitude > 0.001f)
            _boid.RotateTo(dir);
        if (hasLOS)
            TryShoot();
    }
    private Transform FindTarget()
    {
        float chance = Random.value;
        if(chance < 0.5f)
        {
            var enemyLeader = LeaderManager.instance.GetLeader(_boid.Leader);
            if (enemyLeader != null &&LineOfSight.IsOnSight(_boid.transform.position, enemyLeader.transform.position))
            {
                return enemyLeader.transform;
            }
        }
        var allBoids = BoidManager.instance.GetBoids;
        foreach (var boid in allBoids)
        {
            if (!LineOfSight.IsOnSight(_boid.transform.position, boid.transform.position)) continue;
            if (boid.typeBoid != _boid.typeBoid)
                return boid.transform;
        }
        return null;
    }
    private void TryShoot()
    {
        if (Time.time - _currentCooldown < _maxCooldown)
            return;
        var bullet = PoolBullet.instance.GetBullet();
        bullet.transform.position = _gunSight.position;
        bullet.transform.rotation = _gunSight.rotation;
        bullet.GetComponent<Bullet>().Shoot(_boid.typeBoid);
        _currentCooldown = Time.time;
    }
}
