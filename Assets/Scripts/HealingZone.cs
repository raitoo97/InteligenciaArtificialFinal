using UnityEngine;
public enum HealingZoneTeam
{
    Blue,
    Violet
}
public class HealingZone : MonoBehaviour
{
    [SerializeField] private HealingZoneTeam zoneType;
    [SerializeField] private float healAmount = 60f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            var boid = other.GetComponent<Boid>();
            if (boid == null) return;
            if (IsCorrectTeam(boid.typeBoid))
            {
                boid.Life.Heal(healAmount);
            }
        }
    }
    private bool IsCorrectTeam(TypeBoid boidType)
    {
        if (zoneType == HealingZoneTeam.Blue && boidType == TypeBoid.BlueTeam)
            return true;
        if (zoneType == HealingZoneTeam.Violet && boidType == TypeBoid.VioletTeam)
            return true;
        return false;
    }
}
