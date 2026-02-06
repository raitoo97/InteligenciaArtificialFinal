using UnityEngine;
public static class LineOfSight
{
    public static bool IsOnSight(Vector3 startPos, Vector3 targetPos)
    {
        Vector3 direction = targetPos - startPos;
        float distance = direction.magnitude;
        Vector3 origin = startPos + Vector3.up;
        Debug.DrawRay(origin, direction.normalized * distance, Color.red, 2f);
        return !Physics.Raycast(origin, direction.normalized, distance, LayerMask.GetMask("Wall"));
    }
}
