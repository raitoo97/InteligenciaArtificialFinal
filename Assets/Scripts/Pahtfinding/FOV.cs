using UnityEngine;
public static class FOV
{
    public static bool InFieldOfView(Transform target, Transform originPosition, float viewRadius, float viewAngle)
    {
        var directionToTarget = target.position - originPosition.position;
        if(directionToTarget.magnitude <= viewRadius)
        {
            var angleToTarget = Vector3.Angle(originPosition.forward, directionToTarget);
            if(angleToTarget < viewAngle * 0.5f)
            {
                return LineOfSight.IsOnSight(originPosition.position, target.position);
            }
        }
        return false;
    }
}
