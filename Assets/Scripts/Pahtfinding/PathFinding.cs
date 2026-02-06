using System.Collections.Generic;
using UnityEngine;
public static class PathFinding
{
    public static List<Vector3> CalculateAStart(Vector3 initPos, Vector3 endPos)
    {
        Node Start = NodeManager.GetClosetNode(initPos);
        Node end = NodeManager.GetClosetNode(endPos);
        var frontier = new PriorityQueue<Node>();
        var cameFrom = new Dictionary<Node, Node>();
        var costSoFar = new Dictionary<Node, float>();
        frontier.Enqueue(Start, 0);
        cameFrom.Add(Start, null);
        costSoFar.Add(Start, 0);
        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            if (current == end)
            {
                List<Vector3> path = new List<Vector3>();
                while (current != null)
                {
                    path.Add(current.transform.position);
                    current = cameFrom[current];
                }
                path.Reverse();
                path.Add(endPos);
                return path;
            }
            foreach (Node node in current.Neighbords)
            {
                var newCost = costSoFar[current] + node.cost;
                var valueDistance = Vector3.Distance(node.transform.position, endPos);
                var priority = newCost + valueDistance;
                if (!cameFrom.ContainsKey(node))
                {
                    frontier.Enqueue(node, priority);
                    cameFrom.Add(node, current);
                    costSoFar.Add(node, newCost);
                }
                else if (newCost < costSoFar[node])
                {
                    cameFrom[node] = current;
                    costSoFar[node] = newCost;
                    frontier.Enqueue(node, priority);
                }
            }
        }
        return new List<Vector3>();
    }
    public static List<Vector3> CalculateTheta(Vector3 initPos, Vector3 endPos)
    {
        var aStart = CalculateAStart(initPos, endPos);
        int current = 0;
        while(current + 2 < aStart.Count)
        {
            if (LineOfSight.IsOnSight(aStart[current], aStart[current + 2]))
                aStart.RemoveAt(current + 1);
            else
                current++;
        }
        return aStart;
    }
}
