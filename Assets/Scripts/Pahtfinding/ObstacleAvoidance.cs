using UnityEngine;
[RequireComponent(typeof(Agent))]
public class ObstacleAvoidance : MonoBehaviour
{
    private Agent _agent;
    [SerializeField] private float radius;
    private void Awake()
    {
        _agent = this.GetComponent<Agent>();
    }
    private void Update()
    {
        _agent.AddForce(ApplyObstacleAvoidance() * _agent.ObstacleAvoidanceWeight);
    }
    private Vector3 ApplyObstacleAvoidance()
    {
        Vector3 position = transform.position;
        Vector3 direction = transform.forward;
        float distance = _agent.Velocity.magnitude;
        if(Physics.SphereCast(position, radius, direction,out RaycastHit hit, distance, LayerMask.GetMask("ObstacleAvoidance")))
        {
            var obstacle = hit.transform;
            Vector3 dirToObject = obstacle.position - position;
            float angle = Vector3.SignedAngle(direction, dirToObject, Vector3.up);
            var desired = angle >=0 ? -transform.right : transform.right;
            desired *= _agent.MaxSpeed;
            var steering = desired - _agent.Velocity;
            steering = Vector3.ClampMagnitude(steering, _agent.MaxForce);
            return steering;
        }
        return Vector3.zero;
    }
    private void OnDrawGizmos()
    {
        if (_agent == null) return;
        Gizmos.color = Color.cyan;
        Vector3 pos = transform.position;
        Vector3 dir = transform.forward;
        float distance = _agent.MaxSpeed;
        Gizmos.DrawWireSphere(pos, radius);
        Vector3 endPos = pos + dir * distance;
        Gizmos.DrawWireSphere(endPos, radius);
    }
}
