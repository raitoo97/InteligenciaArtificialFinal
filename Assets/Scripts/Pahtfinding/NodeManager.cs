using System.Collections.Generic;
using UnityEngine;
public static class NodeManager
{
    private static List<Node> _totalNodes = new List<Node>();
    public static float _maxDistanceNeighbord = 20;
    public static void RegisterNode(Node node)
    {
        if (!_totalNodes.Contains(node))
            _totalNodes.Add(node);
    }
    public static void UnregisterNode(Node node)
    {
        if (_totalNodes.Contains(node))
            _totalNodes.Remove(node);
    }
    public static void CompleteNeighboards()
    {
        if (_totalNodes.Count <= 0) return;
        foreach (var node in _totalNodes)
        {
            foreach (var otherNode in _totalNodes)
            {
                if (node == otherNode) continue;
                if (LineOfSight.IsOnSight(node.transform.position,otherNode.transform.position))
                {
                    if(Vector3.Distance(node.transform.position,otherNode.transform.position) < _maxDistanceNeighbord)
                        node.AddNeighbord(otherNode);
                }
            }
        }
    }
    public static Node GetClosetNode(Vector3 nearPos)
    {
        float minDistance = Mathf.Infinity;
        Node closestNode = null;
        foreach (var node in _totalNodes)
        {
            float distance = Vector3.Distance(nearPos, node.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestNode = node;
            }
        }
        return closestNode;
    }
}
