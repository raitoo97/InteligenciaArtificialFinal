using UnityEngine;
public class AttackBoidState : IState
{
    private Boid _boid;
    private Agent _agent;
    private FSM _fsm;
    private Transform _target;
    private Transform _gunSight;
    private float _shootRange = 5f;
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
        _target = _boid.GetClosestVisibleEnemy();
        _maxCooldown = 3;
        _currentCooldown = Time.time;
        _isStopped = false;
        _boid.ClearPath();
        _agent.ChangeMove(true);
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
        _leaderTooClose = _boid.HasLeaderTooClose();
        _boid.CheckHasNeighbors(ref _hasNeighbors);
        _boid.CheckHasEnemyNeighbors(ref _hasEnemyNearby, _shootRange);
        bool shouldMove = _hasNeighbors || _leaderTooClose || _hasEnemyNearby;
        if (_target != null)
        {
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
                    _boid.ApplyEnemySeparation(_shootRange);
            }
        }
        else
        {
            _agent.ChangeMove(true);
            _isStopped = false;
        }
        if (dir.sqrMagnitude > 0.001f)
            _boid.RotateTo(dir);
        TryShoot();
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
