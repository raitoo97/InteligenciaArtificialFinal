using UnityEngine;
public class AttackLeaderState : IState
{
    private Agent _agent;
    private Leader _leader;
    private Transform _target;
    private Transform _gunSight;
    private float _maxCooldown;
    private float _currentCooldown;
    private FSM _fsm;
    public AttackLeaderState(Leader leader, Agent agent,Transform gunSight,FSM fsm)
    {
        _leader = leader;
        _gunSight = gunSight;
        _agent = agent;
        _fsm = fsm;
    }
    public void OnEnter()
    {
        _target = _leader.GetClosestVisibleEnemy();
        _maxCooldown = 3;
        _currentCooldown = Time.time;
    }
    public void OnExit()
    {
        _agent.ChangeMove(true);
    }
    public void OnUpdate()
    {
        if (_target == null)
        {
            _fsm.ChangeState(FSM.State.SearchEnemy);
            return;
        }
        Vector3 dir = _target.position - _leader.transform.position;
        _agent.ChangeMove(false);
        if (dir.sqrMagnitude > 0.001f)
        {
            _leader.RotateTo(dir);
        }
        TryShoot();
    }
    private void TryShoot()
    {
        if (_gunSight == null) return;
        if (Time.time - _currentCooldown < _maxCooldown) return;
        var bullet = PoolBullet.instance.GetBullet();
        bullet.transform.position = _gunSight.position;
        bullet.transform.rotation = _gunSight.rotation;
        bullet.GetComponent<Bullet>().Shoot(_leader.IsVioletLeader ? TypeBoid.VioletTeam : TypeBoid.BlueTeam);
        _currentCooldown = Time.time;
    }
}
