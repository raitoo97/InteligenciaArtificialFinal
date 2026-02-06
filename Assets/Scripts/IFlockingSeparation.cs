using System.Collections.Generic;
using UnityEngine;
public interface IFlockingSeparation
{
    public abstract Vector3 Separation(List<Boid>boids,float range);
}
