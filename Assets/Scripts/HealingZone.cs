using UnityEngine;
public class HealingZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            var boid = other.GetComponent<Boid>();
            if (boid == null) return;
            boid.Life.Heal(60);
        }
        if (other.gameObject.layer == 10)
        {
            var leader = other.GetComponent<Leader>();
            if (leader == null) return;
            leader.Life.Heal(60);
        }
    }
}
