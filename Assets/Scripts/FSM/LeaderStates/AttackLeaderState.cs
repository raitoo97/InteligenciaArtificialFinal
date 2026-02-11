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
    private GameObject _bullet;
    private float _bulletSpeed;
    public AttackLeaderState(GameObject bullet, Transform gunSight, Leader leader, Agent agent,FSM fsm)
    {
        _leader = leader;
        _gunSight = gunSight;
        _agent = agent;
        _fsm = fsm;
        _bullet = bullet;
        _bulletSpeed = _bullet.GetComponent<Bullet>().GetSpeed;
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
        if (_leader.Life.GetLife <= _leader.MinLifeToRetreat)
        {
            _leader.ClearPath();
            _fsm.ChangeState(FSM.State.Retreat);
            return;
        }
        if (_target == null)
        {
            _fsm.ChangeState(FSM.State.SearchEnemy);
            return;
        }
        bool visible = FOV.InFieldOfView(_target, _leader.transform, _leader.ViewRadius, _leader.ViewAngle);
        if (!visible)
        {
            _fsm.ChangeState(FSM.State.SearchEnemy);
            return;
        }
        _agent.ChangeMove(false);
        RotateWhitPrediction(_target.GetComponent<Agent>());
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
    public void RotateWhitPrediction(Agent Target)
    {
        Vector3 dir = Target.transform.position - _leader.transform.position;
        float distance = dir.magnitude;
        float predictionTime = distance / _bulletSpeed;
        Vector3 aim = Target.transform.position + Target.Velocity * predictionTime;
        Vector3 directionToAim = (aim - _leader.transform.position).normalized;
        if (directionToAim != Vector3.zero)
        {
            Quaternion desiredRot = Quaternion.LookRotation(directionToAim);
            _leader.transform.rotation = Quaternion.RotateTowards(_leader.transform.rotation, desiredRot, 360f * Time.deltaTime);
        }
    }
}
