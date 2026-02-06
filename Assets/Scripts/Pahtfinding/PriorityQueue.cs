using System.Collections.Generic;
using UnityEngine;
public class PriorityQueue <T>
{
    Dictionary<T,float> _allElements = new Dictionary<T,float>();
    public int Count => _allElements.Count;
    public void Enqueue (T element, float cost)
    {
        if (_allElements.ContainsKey(element))
            _allElements.Add(element, cost);
        else
            _allElements[element] = cost;
    }
    public T Dequeue ()
    {
        T minElement = default;
        float minCost = Mathf.Infinity;
        foreach (var element in _allElements)
        {
            if (element.Value < minCost)
            {
                minCost = element.Value;
                minElement = element.Key;
            }
        }
        _allElements.Remove(minElement);
        return minElement;
    }
}
